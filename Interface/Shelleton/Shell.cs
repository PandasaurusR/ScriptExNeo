﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ScriptExNeo.Interface.Shelleton {
    public static class Shell {
        // Shell command namespace
        const string _commandNamespace = "ScriptExNeo.Interface.Shelleton.Commands";
        static Dictionary<string, Dictionary<string, IEnumerable<ParameterInfo>>> _commandLibraries;

        public static void Initialise() {
            // Any static classes containing commands for use from the 
            // console are located in the Commands namespace. Load 
            // references to each type in that namespace via reflection:
            _commandLibraries = new Dictionary<string, Dictionary<string,
                    IEnumerable<ParameterInfo>>>();

            // Use reflection to load all of the classes in the Commands namespace:
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == _commandNamespace
                    select t;
            var commandClasses = q.ToList();

            foreach (var commandClass in commandClasses) {
                // Load the method info from each class into a dictionary:
                var methods = commandClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
                var methodDictionary = new Dictionary<string, IEnumerable<ParameterInfo>>();
                foreach (var method in methods) {
                    string commandName = method.Name;
                    methodDictionary.Add(commandName, method.GetParameters());
                }
                // Add the dictionary of methods for the current class into a dictionary of command classes:
                _commandLibraries.Add(commandClass.Name, methodDictionary);
            }
        }

        public static void Run() {
            string _input = Console.ReadLine();
            Run(_input);
        }
        public static void Run(List<string> commands) {
            foreach (var command in commands) {
                Run(command);
            }
        }
        public static void Run(string command) {
            // Ignore if input is empty
            if (string.IsNullOrWhiteSpace(command)) {
                return;
            }

            // Attempt to parse input
            try {
                ShellCommand _cmd = new ShellCommand(command);

                string result = Execute(_cmd);

                WriteLine(result);
            }
            catch (Exception ex) {
                WriteLine(" EXCEPTION: " + ex.Message);
                WriteLine("STACKTRACE: \n" + ex.StackTrace);
            }
        }

        static string Execute(ShellCommand command) {
            // Validate the command name:
            if (!_commandLibraries.ContainsKey(command.LibraryClassName)) {
                return BadCommandMessage();
            }
            var methodDictionary = _commandLibraries[command.LibraryClassName];
            if (!methodDictionary.ContainsKey(command.Name)) {
                return BadCommandMessage();
            }

            // Make sure the corret number of required arguments are provided:
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            var methodParameterValueList = new List<object>();
            IEnumerable<ParameterInfo> paramInfoList = methodDictionary[command.Name].ToList();

            // Validate proper # of required arguments provided. Some may be optional:
            var requiredParams = paramInfoList.Where(p => p.IsOptional == false);
            var optionalParams = paramInfoList.Where(p => p.IsOptional == true);
            int requiredCount = requiredParams.Count();
            int optionalCount = optionalParams.Count();
            int providedCount = command.Arguments.Count();

            List<string> _args = new List<string>(ParameterInfoToStringList(optionalParams));
            _args.AddRange(ParameterInfoToStringList(requiredParams));
            if (requiredCount > providedCount) {
                return string.Format(
                    "Missing required arguments ({0} required, {1} optional, {2} provided)\n --> {3}.{4} {5}",
                    requiredCount, optionalCount, providedCount, command.LibraryClassName, command.Name, string.Join(" ", _args));
            }
            if (providedCount > (requiredCount + optionalCount)) {
                return string.Format(
                    "Too many arguments provided ({0} required, {1} optional, {2} provided)\n --> {3}.{4} {5}",
                    requiredCount, optionalCount, providedCount, command.LibraryClassName, command.Name, string.Join(" ", _args));
            }

            // Make sure all arguments are coerced to the proper type, and that there is a 
            // value for every emthod parameter. The InvokeMember method fails if the number 
            // of arguments provided does not match the number of parameters in the 
            // method signature, even if some are optional:
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            if (paramInfoList.Count() > 0) {
                // Populate the list with default values:
                foreach (var param in paramInfoList) {
                    // This will either add a null object reference if the param is required 
                    // by the method, or will set a default value for optional parameters. in 
                    // any case, there will be a value or null for each method argument 
                    // in the method signature:
                    methodParameterValueList.Add(param.DefaultValue);
                }

                // Now walk through all the arguments passed from the console and assign 
                // accordingly. Any optional arguments not provided have already been set to 
                // the default specified by the method signature:
                for (int i = 0; i < command.Arguments.Count(); i++) {
                    var methodParam = paramInfoList.ElementAt(i);
                    var typeRequired = methodParam.ParameterType;
                    object value = null;
                    try {
                        // Coming from the Console, all of our arguments are passed in as 
                        // strings. Coerce to the type to match the method paramter:
                        value = CoerceArgument(typeRequired, command.Arguments.ElementAt(i));
                        methodParameterValueList.RemoveAt(i);
                        methodParameterValueList.Insert(i, value);
                    }
                    catch (ArgumentException) {
                        string argumentName = methodParam.Name;
                        string argumentTypeName = typeRequired.Name;
                        string message =
                            string.Format(""
                            + "Argument '{0}' contains value '{1}' that cannot be parsed to type '{2}'",
                            argumentName, command.Arguments.ElementAt(i), argumentTypeName);
                        throw new ArgumentException(message);
                    }
                }
            }


            // Set up to invoke the method using reflection:
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            Assembly current = typeof(Program).Assembly;

            // Need the full Namespace for this:
            Type commandLibaryClass = current.GetType(_commandNamespace + "." + command.LibraryClassName);

            object[] inputArgs = null;
            if (methodParameterValueList.Count > 0) {
                inputArgs = methodParameterValueList.ToArray();
            }
            var typeInfo = commandLibaryClass;

            // This will throw if the number of arguments provided does not match the number 
            // required by the method signature, even if some are optional:
            try {
                var result = typeInfo.InvokeMember(
                    command.Name,
                    BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                    null, null, inputArgs);

                // For some reason, this is required to stop Visual Studio from screaming
                if (result is null) {
                    return "";
                }

                try {
                    return result.ToString();
                }
                catch (NullReferenceException) {
                    // Return an empty string on null response from function
                    return "";
                }
            }
            catch (TargetInvocationException ex) {
                throw ex.InnerException;
            }
        }


        static object CoerceArgument(Type requiredType, string inputValue) {
            var requiredTypeCode = Type.GetTypeCode(requiredType);
            string exceptionMessage =
                string.Format("Cannnot coerce the input argument {0} to required type {1}",
                inputValue, requiredType.Name);

            object result;
            switch (requiredTypeCode) {
                case TypeCode.String:
                    result = inputValue;
                    break;
                case TypeCode.Int16:
                    short number16;
                    if (Int16.TryParse(inputValue, out number16)) {
                        result = number16;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Int32:
                    int number32;
                    if (Int32.TryParse(inputValue, out number32)) {
                        result = number32;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Int64:
                    long number64;
                    if (Int64.TryParse(inputValue, out number64)) {
                        result = number64;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Boolean:
                    bool trueFalse;
                    if (bool.TryParse(inputValue, out trueFalse)) {
                        result = trueFalse;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Byte:
                    byte byteValue;
                    if (byte.TryParse(inputValue, out byteValue)) {
                        result = byteValue;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Char:
                    char charValue;
                    if (char.TryParse(inputValue, out charValue)) {
                        result = charValue;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.DateTime:
                    DateTime dateValue;
                    if (DateTime.TryParse(inputValue, out dateValue)) {
                        result = dateValue;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Decimal:
                    Decimal decimalValue;
                    if (Decimal.TryParse(inputValue, out decimalValue)) {
                        result = decimalValue;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Double:
                    Double doubleValue;
                    if (Double.TryParse(inputValue, out doubleValue)) {
                        result = doubleValue;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.Single:
                    Single singleValue;
                    if (Single.TryParse(inputValue, out singleValue)) {
                        result = singleValue;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.UInt16:
                    UInt16 uInt16Value;
                    if (UInt16.TryParse(inputValue, out uInt16Value)) {
                        result = uInt16Value;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.UInt32:
                    UInt32 uInt32Value;
                    if (UInt32.TryParse(inputValue, out uInt32Value)) {
                        result = uInt32Value;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                case TypeCode.UInt64:
                    UInt64 uInt64Value;
                    if (UInt64.TryParse(inputValue, out uInt64Value)) {
                        result = uInt64Value;
                    }
                    else {
                        throw new ArgumentException(exceptionMessage);
                    }
                    break;
                default:
                    throw new ArgumentException(exceptionMessage);
            }
            return result;
        }


        static string BadCommandMessage() {
            // TODO: Update message to include command
            return string.Format("Command does not exist.");
        }

        static List<string> ParameterInfoToStringList(IEnumerable<ParameterInfo> parameterList) {
            List<string> ls = new List<string>();

            foreach (var item in parameterList) {
                if (item.IsOptional) {
                    ls.Add($"[{item}]");
                }
                else {
                    ls.Add($"<{item}>");
                }
            }

            return ls;
        }

        public static List<CommandHelpInfo> GetCommands() {
            List<CommandHelpInfo> _commands = new List<CommandHelpInfo>();

            foreach (var _class in _commandLibraries.Keys) {
                foreach (var _command in _commandLibraries[_class].Keys) {
                    CommandHelpInfo _chi = new CommandHelpInfo {
                        Name = string.Format("{0}.{1}", _class, _command)
                    };

                    IEnumerable<ParameterInfo> _params = _commandLibraries[_class][_command].ToList();
                    foreach (var _param in _params) {
                        if (_param.IsOptional) {
                            _chi.OptionalArgs.Add(string.Format("[{0}]", _param.ToString()));
                        }
                        else {
                            _chi.RequiredArgs.Add(string.Format("<{0}>", _param.ToString()));
                        }
                    }
                    _commands.Add(_chi);
                }
            }

            return _commands;
        }
        public class CommandHelpInfo {
            public string Name { get; set; }
            public List<string> RequiredArgs { get; set; }
            public List<string> OptionalArgs { get; set; }
            public CommandHelpInfo() {
                RequiredArgs = new List<string>();
                OptionalArgs = new List<string>();
            }
        }


        /// <summary>
        /// Write string to console
        /// </summary>
        public static void WriteLine(string line = "") {
            if (line.Length > 0) {
                Console.WriteLine(line);
            }
        }


        const string Prompt = "> ";
        /// <summary>
        /// Read string from console
        /// </summary>
        public static string ReadLine(string line = "") {
            Console.Write(Prompt + line);
            return Console.ReadLine();
        }
    }
}

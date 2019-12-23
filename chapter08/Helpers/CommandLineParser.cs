using System;
using System.Linq;

namespace chapter08.Helpers
{
    public static class CommandLineParser
    {
        public static T ParseArguments<T>(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length == 0)
            {
                Console.WriteLine("No arguments passed in - using defaults");

                return Activator.CreateInstance<T>();
            }

            if (args.Length % 2 != 0)
            {
                throw new ArgumentException($"Arguments must be in pairs, there were {args.Length} passed in");
            }

            var argumentObject = Activator.CreateInstance<T>();

            var properties = argumentObject.GetType().GetProperties();

            for (var x = 0; x < args.Length; x += 2)
            {
                var property = properties.FirstOrDefault(a => a.Name.Equals(args[x], StringComparison.CurrentCultureIgnoreCase));

                if (property == null)
                {
                    Console.WriteLine($"{args[x]} is an invalid argument");

                    continue;
                }

                if (property.PropertyType.IsEnum)
                {
                    property.SetValue(argumentObject, Enum.Parse(property.PropertyType, args[x + 1], true));
                }
                else
                {
                    property.SetValue(argumentObject, args[x + 1]);
                }
            }

            return argumentObject;
        }
    }
}
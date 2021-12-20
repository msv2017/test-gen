using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TestAppService;
using TestGenerator.Templates;

namespace TestGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = AssemblyDefinition.ReadAssembly(typeof(AppService).Assembly.Location);

            var sut = assembly.MainModule.GetType("TestAppService.AppService");

            var fields = sut.Fields
                .Where(x => x.Attributes.HasFlag(FieldAttributes.Private | FieldAttributes.InitOnly))
                .Where(x => x.FieldType.Resolve().IsInterface)
                .ToList();

            NewSection();

            var services = new HashSet<string>();
            var methods = new Dictionary<MethodDto, List<CallDto>>();
            var map = sut.Methods.ToDictionary(x => x.FullName, x => x.Body);

            var publicMethods = sut.Methods
                .Where(x => x.IsPublic)
                .Where(x => !x.IsConstructor)
                .Where(x => !(x.IsGetter || x.IsSetter))
                .ToList();

            foreach (var def in publicMethods)
            {
                var methodCalls = new HashSet<string>();
                Traverse(map, methodCalls, def.FullName);
                services.UnionWith(methodCalls.Select(GetTypeAndName).Select(x => x.type));
                var (type, name) = GetTypeAndName(def.FullName);
                methods.Add(
                    new MethodDto { name = name.Split("(")[0], fullName = name, type = type },
                    methodCalls
                    .Select(GetTypeAndName)
                    .Select(x => new CallDto
                    {
                        method = x.name,
                        service = "_" + ToCamelCase(x.type)
                    }).ToList());
            }


            var template = new Template(
                sut.Name,
                methods,
                services.Select(x => new ServiceDto { name = ToCamelCase(x), type = x }).ToList());
            var content = template.TransformText();
            //File.WriteAllText("Test.cs", content);
            Console.WriteLine(content);
        }

        static (string type, string name) GetTypeAndName(string text)
        {
            // System.Void TestAppService.IAppService2::Bar()
            text = text.Split(" ").Skip(1).First();
            var arr = text.Split("::");
            return (
                arr[0].Split(".").Last(),
                arr[1]);
        }

        static string ToCamelCase(string name)
        {
            name = name.TrimStart('I');
            return Char.ToLowerInvariant(name[0]) + name.Substring(1);
        }

        static void Traverse(Dictionary<string, MethodBody> methods, HashSet<string> methodCalls, string fullName)
        {
            var body = methods[fullName];
            foreach (var instruction in body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Callvirt)
                {
                    methodCalls.Add(instruction.Operand.ToString());
                }
                if (instruction.OpCode == OpCodes.Call)
                {
                    Traverse(methods, methodCalls, instruction.Operand.ToString());
                }
            }
        }

        static void NewSection()
        {
            Console.WriteLine("".PadRight(80, '='));
        }
    }
}

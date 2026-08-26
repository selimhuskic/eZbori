using System.Reflection;
using System.Text;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace Migrations.SetUp
{
    internal class SqlScriptAndCodeScriptProvider : IScriptProvider
    {
        private readonly Assembly _assembly;
        private readonly string _scriptsDirectory;

        public SqlScriptAndCodeScriptProvider(Assembly assembly, string scriptsDirectory)
        {
            _assembly = assembly;
            _scriptsDirectory = scriptsDirectory;
        }

        public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
        {
            return GetSqlScripts()
                    .Concat(GetCodeScripts(connectionManager))
                    .OrderBy(x => x.Name)
                    .ToList();
        }

        //Question: Why not just use the EmbeddedScriptAndCodeProvider included in DbUp?
        //Answer: The class is not really extensible and we need to plugin a different EmbeddedScriptsProvider so the namespace would not be included in the name of the sql scripts (see GetSqlScripts method blow)
        private IEnumerable<SqlScript> GetCodeScripts(IConnectionManager connectionManager)
        {
            var script = typeof(IScript);
            return connectionManager.ExecuteCommandsWithManagedConnection(dbCommandFactory => _assembly
                .GetTypes()
                .Where(type => script.IsAssignableFrom(type) && type.GetTypeInfo().IsClass)
                .Select(s => (SqlScript)new LazySqlScript(s.Name, () => ((IScript)Activator.CreateInstance(s))?.ProvideScript(dbCommandFactory)))
                .ToList());
        }

        //Question: Why not just use the EmbeddedScriptsProvider included in DbUp?
        //Answer: It would work but that EmbeddedScriptsProvider includes the namespace for the sql scripts.
        //This is annoying since our custom code just uses the filename (without namespace).
        //Since we were already getting the scripts ourselves from the directory, I just kept this way of working
        private IEnumerable<SqlScript> GetSqlScripts()
        {
            var assemblyDirectory = Path.GetDirectoryName(_assembly.Location);
            Console.WriteLine(assemblyDirectory);
            return GetSqlScriptsFromDirectory(Path.Combine(assemblyDirectory ?? throw new InvalidOperationException(), _scriptsDirectory)).ToList();
        }

        private static IEnumerable<SqlScript> GetSqlScriptsFromDirectory(string fullPathToScripts)
        {
            if (!Directory.Exists(fullPathToScripts))
                yield break;

            var files = Directory.GetFiles(fullPathToScripts).OrderBy(fileName => fileName);
            foreach (var file in files)
            {
                yield return SqlScript.FromFile(file, Encoding.UTF8);
            }
        }
    }
}

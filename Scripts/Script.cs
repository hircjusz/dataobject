using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using SoftwareMind.Shared.Utils;
using log4net;

namespace SoftwareMind.Scripts
{
    /// <summary>
    /// Klasa reprezentujaca skrypt
    /// </summary>
    /// <remarks>
    /// Przyk³¹d urzycia
    /// <example>
    /// <code>
    /// string code = @"
    /// declarations
    /// {
    /// int Add(int a, int b)
    /// {
    /// return a + b;
    /// }
    /// }
    /// result = Add(a + b, c);
    /// ";
    /// Script script = new Script(code);
    /// script["a"] = 1;
    /// script["b"] = 2;
    /// script["c"] = 3;
    /// script.Execute();
    /// Console.WriteLine(script["result"]);
    /// </code>
    /// </example>
    /// Przyk³ad u¿ycia zawierajacy blok usings:
    /// <example>
    /// <code>
    /// string code = @"
    /// usings
    /// {
    /// using System.Diagnostics;
    /// }
    /// declarations
    /// {
    /// int Add(int a, int b)
    /// {
    /// return a + b;
    /// }
    /// }
    /// Stopwatch stopwatch = new Stopwatch();
    /// stopwatch.Start();
    /// result = Add(a, b);
    /// stopwatch.Stop();
    /// time = stopwatch.ElapsedMilliseconds;
    /// ";
    /// Script script = new Script(code);
    /// script.ReferencedAssemblies.Add("System.dll");
    /// script["a"] = 1;
    /// script["b"] = 2;
    /// script["time"] = 0L;
    /// script.Execute();
    /// </code>
    /// </example>
    /// Przyk³ad u¿ycia zawierajacy blok usings:
    /// <example>
    /// <code>
    /// string code = @"
    /// usings
    /// {
    ///     using System.Drawing;
    /// }
    ///
    /// assemblies
    /// {
    ///      System.Drawing.dll;
    /// }
    ///
    /// Point p = new Point(1,2);
    /// result = p.X &lt; p.Y;
    /// ";
    /// Script script = new Script(code);
    /// script.Execute();
    /// </code>
    /// </example>
    /// </remarks>
    public class Script : SoftwareMind.Scripts.IScript
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Script));

        private static RPNExpression.ExprEnvironment RNPEnviroment = null;
        /// <summary>
        /// Cache na skrypty
        /// </summary>
        private static Dictionary<string, List<RPNExpression.RPNObject>> cacheRNP = new Dictionary<string, List<RPNExpression.RPNObject>>();

        private static Dictionary<string, CompiledScriptWithAppDomainWraper> cache = new Dictionary<string, CompiledScriptWithAppDomainWraper>();
        /// <summary>
        /// Rozmiar cache.
        /// </summary>
        private static int cacheSize = 1000;
        /// <summary>
        /// Wyrazênie regularne do wykrywania komentarzy
        /// </summary>
        private static Regex commentRegex = new Regex(@"(//.*?(\r\n|$))  |  (/\*(.|\n)*?\*/)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
        /// <summary>
        /// Skompilowany skrypt.
        /// </summary>
        private CompiledScript compiledScript;
        /// <summary>
        /// Regularne wyra¿enie pasujace do bloku usings.
        /// </summary>
        private static Regex decratationRegex = CreateBalandedGroupRegex("declarations");
        /// <summary>
        /// Tekst bloku deklaracji.
        /// </summary>
        private string methodDeclaration;
        /// <summary>
        /// Kolejka cache.
        /// </summary>
        private static LinkedList<string> queue = new LinkedList<string>();
        /// <summary>
        /// Skrypt po usuniêciu bloku declarations i usings.
        /// </summary>
        private string script;
        /// <summary>
        /// Liczba skryptów na AppDomain.
        /// </summary>
        private static int scriptsPerAppDomain = 50;
        /// <summary>
        /// Tekst bloku using.
        /// </summary>
        private string usings;
        /// <summary>
        /// Wyra¿enie regularne reprezentuj¹ce blok usings.
        /// </summary>
        private static Regex usingsRegex = CreateBalandedGroupRegex("usings");
        /// <summary>
        /// S³ownik nazwa zmiennej-> wartoœæ zmiennej.
        /// </summary>
        private IDictionary<string, object> values;
        /// <summary>
        /// Wyra¿enie regularne reprezentuj¹ce blok usings.
        /// </summary>
        private static Regex assembliesRegex = CreateBalandedGroupRegex("assemblies");
        /// <summary>
        /// Lista assembly jakie zostan¹ do³¹czone w czasie kompliacji(podawane w skrypcie przez u¿ystkownika).
        /// </summary>
        private string[] assembliesToRefenrece = new string[0];

        /// <summary>
        /// Tworzy now¹ instancjê klasy <see cref="Script"/>.
        /// </summary>
        /// <param name="inputScript">Kod Ÿród³owy skryptu.</param>
        /// <exception cref="System.ArgumentException">Jest rzucany jeœli <paramref name="name"/> jest null albo pusty.</exception>
        /// <exception cref="Workflow.Scripts.CompilerErrorException">Jest rzucany jeœli skrypt jest niepoprawny.</exception>
        ///

        private bool evaluateOnly;

        public Script(string inputScript)
        {
            if (String.IsNullOrEmpty(inputScript))
                throw new ArgumentException(ResX.GetFormattedString("NullOrEmptyErrorMessage", ResX.DefaultLanguage, "inputscript"), "inputScript");

            if (values == null)
                values = new Dictionary<string, object>();
            script = commentRegex.Replace(inputScript, String.Empty);
            if (script[0] == '@')
            {
                evaluateOnly = true;
                script = script.Substring(1, script.Length - 1);
            }
            else
            {



                ReferencedAssemblies = new List<String>();

                string newScript = null;
                Match match = usingsRegex.Match(script);
                if (match.Success)
                {
                    usings = match.Groups["text"].Value;
                    newScript = script.Replace(match.Value, String.Empty);
                }
                else
                {
                    newScript = script;
                    usings = String.Empty;
                }

                match = assembliesRegex.Match(newScript);
                if (match.Success)
                {
                    ReferencedAssemblies = match.Groups["text"].Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).
                        Select(x => x.Trim()).Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
                    newScript = newScript.Replace(match.Value, String.Empty);
                }

                match = decratationRegex.Match(newScript);
                if (match.Success)
                {
                    methodDeclaration = match.Groups["text"].Value;
                    script = newScript.Replace(match.Value, String.Empty);
                }
                else
                {
                    script = newScript;
                    methodDeclaration = String.Empty;
                }
            }
        }

        /// <summary>
        /// Tworzy now¹ instancjê klasy <see cref="Script"/>.
        /// </summary>
        /// <param name="inputScript">Kod Ÿród³owy skryptu.</param>
        /// <param name="enviroment">Zmienne przekazywane do skryptu</param>
        /// <exception cref="System.ArgumentException">Jest rzucany jeœli <paramref name="name"/> jest null albo pusty.</exception>
        /// <exception cref="Workflow.Scripts.CompilerErrorException">Jest rzucany jeœli skrypt jest niepoprawny.</exception>
        public Script(string inputScript, IDictionary<string, object> enviroment)
            : this(inputScript)
        {
            values = enviroment;
        }

        /// <summary>
        /// Pozwala pobraæ lub ustawic wielkosæ cache.
        /// </summary>Rozmiar cache.</value>
        /// <exception cref="System.ArgumentException">jest rzucany jeœli <paramref name="name"/> jest mniejszy ni¿ 1.</exception>
        public static int CacheSize
        {
            get
            {
                return cacheSize;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException(ResX.GetFormattedString("LessTahn1ErrorMessage", ResX.DefaultLanguage, "value"), "value");
                cacheSize = value;
            }
        }

        /// <summary>
        /// Zawiera Assembly, które zostan¹ dodane do ReferencedAssemblies przy generacji kodu.
        /// assembly.</param>
        /// </summary>
        /// <value>Zale¿ne assemby.</value>
        public IList<String> ReferencedAssemblies
        {
            get;
            private set;
        }

        /// <summary>
        /// Pozwala ustawic i pobraæ liczê skryptów przypadajacych na AppDomain
        /// </summary>
        /// <value>Liczba skryptów na AppDomain</value>
        public static int ScriptsPerAppDomain
        {
            get
            {
                return scriptsPerAppDomain;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException(ResX.GetFormattedString("LessTahn1ErrorMessage", ResX.DefaultLanguage, "value"), "value");
                scriptsPerAppDomain = value;
            }
        }

        /// <summary>
        /// Pozwala pobraæ i ustawiæ wartoœæ zmiennej
        /// </summary>
        /// <value>Wartoœc zmiennej</value>
        /// <exception cref="System.ArgumentException">Jest rzucany jeœli <paramref name="name"/> jest pusta, jest nullem albo
        /// zmienna nie zosta³a zaincjalizowana.</exception>
        public object this[string name]
        {
            get
            {
                if (String.IsNullOrEmpty(name))
                    throw new ArgumentException(ResX.GetFormattedString("NullOrEmptyErrorMessage", ResX.DefaultLanguage, "name"), "name");
                return values[name];
            }
            set
            {
                if (String.IsNullOrEmpty(name))
                    throw new ArgumentException(ResX.GetFormattedString("NullOrEmptyErrorMessage", ResX.DefaultLanguage, "name"), "name");
                values[name] = value;
            }
        }

        /// <summary>
        /// Wykonuje skrypt
        /// </summary>
        /// <returns>Zwraca wartoœæ zmiennej 'result'.</returns>
        /// <exception cref="Workflow.Scripts.CompilerErrorException">Jest rzucany jeœli skrypt jest niepoprawny</exception>
        public object Execute()
        {
            if (evaluateOnly) //ONP interpreter
            {
                if (RNPEnviroment == null)
                {
                    lock (typeof(Script))
                    {
                        if (RNPEnviroment == null)
                        {
                            RNPEnviroment = new RPNExpression.ExprEnvironment();
                            RPNExpression.RPNFunctionUtils.RegisterFunctions(RNPEnviroment);
                        }
                    }
                }

                RPNExpression.RPNExpr expr = new RPNExpression.RPNExpr(script);
                expr.SetVariables(values);
                List<RPNExpression.RPNObject> tokens = null;
                lock (cacheRNP)
                {
                    if (!cacheRNP.TryGetValue(script, out tokens))
                    {
                        expr.Prepare();
                        cacheRNP.Add(script, expr.RPNList);
                    }
                    else
                        expr.RPNList = tokens;
                }
                return expr.GetValue();
            }
            else //C# compiled script
            {
                if (compiledScript == null)
                    Compile();

                lock (compiledScript)
                {
                    foreach (string valueName in values.Keys)
                        compiledScript.SetValue(valueName, values[valueName]);
                    try
                    {
                        compiledScript.SetValue("SourceAppDomain", AppDomain.CurrentDomain);
                        compiledScript.Execute();
                    }
                    catch (Exception ex)
                    {
                        log.Error("Execute", ex);
                        throw new ExecutionException(ResX.GetFormattedString("ExecutionErrorMessage", ResX.DefaultLanguage, ex.Message), ex);
                    }
                    finally
                    {
                        values["result"] = compiledScript.GetValue("result");
                        foreach (string valueName in values.Keys.ToList())
                            values[valueName] = compiledScript.GetValue(valueName);
                    }
                    return values["result"];
                }
            }
        }

        /// <summary>
        /// Sprawdza poprawnosæ skryptu
        /// </summary>
        /// <exception cref="Workflow.Scripts.CompilerErrorException">Jest rzucany jeœli skrypt zawiera b³êdy.</exception>
        public void Validate(IDictionary<string, Type> typeDic)
        {
            if (typeDic == null)
                throw new ArgumentException(ResX.GetString("EmptyDictionaryErrorMessage", ResX.DefaultLanguage));
            if (typeDic.Any(x => String.IsNullOrEmpty(x.Key)) || typeDic.Any(x => x.Value == null))
                throw new ArgumentException(ResX.GetString("BadTypeDeclarationInDictionary", ResX.DefaultLanguage));

            if (compiledScript == null)
                Compile(typeDic);
        }

        /// <summary>
        /// Kompiluje skrypt pobieraj¹c typy danych z aktualnych zmiennych w s³owniku.
        /// </summary>
        private void Compile()
        {
            Compile(values.ToDictionary(x => x.Key, x => x.Value != null ? x.Value.GetType() : typeof(object)));
        }

        /// <summary>
        /// Kod Ÿród³owy skompilowanego skryptu.
        /// </summary>
        private string sourceode;

        /// <summary>
        /// Klucz skryptu?
        /// </summary>
        /// <param name="typeDic"></param>
        /// <returns></returns>
        private string ScriptKey(IDictionary<string, Type> typeDic)
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, Type> pair in typeDic)
            {
                sb.Append(pair.Key);
                sb.Append(':');
                sb.Append(pair.Value.Name);
                sb.AppendLine();
            }
            sb.Append(script);
            sb.Append(usings);
            sb.Append(methodDeclaration);
            return sb.ToString();
        }

        /// <summary>
        /// Kompiluje skrypt pobieraj¹c typy danych z s³ownika podanego jako <paramref name="typeDic"/>.
        /// </summary>
        /// <exception cref="Workflow.Scripts.CompilerErrorException">Throws when script contains syntax errors.</exception>
        ///
        private void Compile(IDictionary<string, Type> typeDic)
        {
            lock (cache)
            {
                string scriptKey = ScriptKey(typeDic);
                if (cache.ContainsKey(scriptKey))
                {
                    compiledScript = cache[scriptKey].CompiledScript;
                    LinkedListNode<string> node = queue.Find(scriptKey);
                    if (node.Previous != null)
                    {
                        node = node.Previous;
                        queue.Remove(scriptKey);
                        queue.AddBefore(node, scriptKey);
                    }
                }
                else
                {
                    if (queue.Count >= CacheSize)
                        FreeCache();

                    IList<string> referencedAssemblies;
                    sourceode = GenerateSourceCode(out referencedAssemblies, typeDic);
                    foreach (var asm in assembliesToRefenrece)
                    {
                        Assembly assembly = Assembly.Load(asm);
                        if (!referencedAssemblies.Contains(assembly.Location))
                            referencedAssemblies.Add(assembly.Location);
                    }

                    AppDomainCacheWraper appDomainCacheWraper = cache.Select(x => x.Value.AppDomainCacheWraper)
                        .Distinct()
                        .Where(x => x.ActiveCount < ScriptsPerAppDomain).FirstOrDefault();

                    appDomainCacheWraper = appDomainCacheWraper ?? new AppDomainCacheWraper();

                    cache[scriptKey] = CreateNewCompiledScript(
                        referencedAssemblies
                            .Union(ReferencedAssemblies.Select(x => x.Trim()).Select(x => x.StartsWith("System") ? x + ".dll" : Assembly.Load(x).Location))
                            .ToList(),
                        sourceode,
                        appDomainCacheWraper,
                        scriptKey
                    );

                    queue.AddLast(scriptKey);
                    compiledScript = cache[scriptKey].CompiledScript;
                }
            }
        }

        /// <summary>
        /// Tworzy wyra¿enie regularne bloku.
        /// </summary>
        /// <param name="blockName">nazwa bloku.</param>
        /// <returns>Wyra¿enie regularne.</returns>
        private static Regex CreateBalandedGroupRegex(string blockName)
        {
            String ignore = "(([^{}\"'@]) |  (\"([^\\\\] | \\.)*?\") | @\".*?\" |'.')*";
            StringBuilder core = new StringBuilder();
            core.Append(@"(\s|\n)*{(?<text>");
            core.Append(ignore);
            core.Append(@"(((?<Open>{)");
            core.Append(ignore);
            core.Append(@")+((?<Close-Open>})");
            core.Append(ignore);
            core.Append(@")+)*(?(Open)(?!)))}");
            return new Regex(blockName + core.ToString(), RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
        }

        /// <summary>
        /// Kompiluje skrypt
        /// </summary>
        /// <param name="variableReferencedAssemblies">Za¿ne Assemby.</param>
        /// <param name="sourceode">Kod Ÿród³owy.</param>
        /// <returns>Skompilowany skrypt wraz z wraperem AppDomain</returns>
        /// <exception cref="Workflow.Scripts.CompilerErrorException">JJest rzucany jeœli skrypt zawiera b³edy.</exception>
        private static CompiledScriptWithAppDomainWraper CreateNewCompiledScript(IList<string> referencedAssemblies, string sourceode, AppDomainCacheWraper appDomainCacheWraper, string scriptKey)
        {
            CompiledScriptWithAppDomainWraper compiledScript = appDomainCacheWraper.CreateCompiledScript(sourceode, referencedAssemblies, scriptKey);
            return compiledScript;
        }

        /// <summary>
        /// Zwalnia obiekty z cache
        /// </summary>
        private static void FreeCache()
        {
            Dictionary<string, int> positions = new Dictionary<string, int>();
            LinkedListNode<string> node = queue.First;
            int pos = 1;
            while (node != null)
            {
                positions[node.Value] = pos++;
                node = node.Next;
            }

            AppDomainCacheWraper[] appDomainList = GetSortedAppDomainList(positions);
            int appDomainIndex = 0;

            while (queue.Count >= CacheSize)
            {
                AppDomainCacheWraper toRemove = appDomainList[appDomainIndex++];
                foreach (string key in toRemove.CreatedScripts)
                {
                    cache.Remove(key);
                    queue.Remove(key);
                }
                toRemove.Dispose();
            }
        }

        /// <summary>
        /// Generuje kod Ÿród³owy w C#
        /// </summary>
        /// <param name="referencedAssemblies">Zale¿ne assembly, które bêd¹ musia³y zostaæ dodane przy kompilacji.
        /// Kolekcja jest obliczana na podstawie <paramref name="typeDic"/></param>
        /// <returns>Kod C#</returns>
        private string GenerateSourceCode(out IList<string> referencedAssemblies, IDictionary<string, Type> typeDic)
        {
            typeDic.Add("SourceAppDomain", typeof(AppDomain));

            StringBuilder mainSourceCode = new StringBuilder();
            mainSourceCode.AppendLine("using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Text;\r\nusing SoftwareMind;\r\n");
            mainSourceCode.AppendLine(usings);
            mainSourceCode.AppendLine("[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]");
            mainSourceCode.AppendLine("[Serializable]");
            mainSourceCode.AppendFormat("public class {0} : {1}\r\n{{\r\n", CompiledScript.className, typeof(GeneratedScriptBase).FullName);

            StringBuilder setSb = new StringBuilder();
            setSb.AppendLine("public override void SetValue(string name, object value)\r\n{");
            setSb.AppendLine("\tswitch(name)\r\n\t{");

            StringBuilder getSb = new StringBuilder();
            getSb.AppendLine("public override object GetValue(string name)\r\n{");
            getSb.AppendLine("\tswitch(name)\r\n\t{");

            HashSet<string> referencedAssembliesSet = new HashSet<string>();
            mainSourceCode.Append("\tpublic dynamic result;\r\n");
            getSb.AppendFormat("\t\tcase \"{0}\" : return this.{0}; break;\r\n", "result");
            setSb.AppendFormat("\t\tcase \"{0}\" : this.{0} = ({1})value; break;\r\n", "result", "object");
            foreach (var value in typeDic)
                if (value.Key != "result")
                {

                    Type type = value.Value;
                    AddReferencedAssembly(referencedAssembliesSet, type.Assembly);
                    string typeFullName;
                    string typeFullName2;
                    if (typeof(DynamicScriptObject).FullName == type.FullName)
                    {
                        typeFullName = "dynamic";
                        typeFullName2 = "object";
                    }
                    else
                    {
                        if (type.IsGenericType)
                            typeFullName2 = typeFullName = BuildGenericName(type, referencedAssembliesSet);
                        else
                            typeFullName2 = typeFullName = type.FullName;
                    }

                    mainSourceCode.AppendFormat("\tpublic {0} {1};\r\n", typeFullName, value.Key);
                    getSb.AppendFormat("\t\tcase \"{0}\" : return this.{0}; break;\r\n", value.Key);
                    setSb.AppendFormat("\t\tcase \"{0}\" : this.{0} = ({1})value; break;\r\n", value.Key, typeFullName2);
                }
            referencedAssembliesSet.Add(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.Location);


            setSb.AppendLine("\t\tdefault: throw new ArgumentException(\"'name' do not exist\", name);");
            setSb.AppendLine("\t}\r\n}");
            getSb.AppendLine("\t\tdefault: throw new ArgumentException(\"'name' do not exist\", name);");
            getSb.AppendLine("\t}\r\n}");

            mainSourceCode.AppendLine(methodDeclaration);

            mainSourceCode.AppendLine("\tpublic override void Execute()\r\n{");
            mainSourceCode.AppendLine("\t" + script);
            mainSourceCode.AppendLine("\t}");
            mainSourceCode.AppendLine("\t" + setSb.ToString());
            mainSourceCode.AppendLine("\t" + getSb.ToString());
            mainSourceCode.AppendLine("\t}");
            referencedAssemblies = referencedAssembliesSet.ToList();

            return mainSourceCode.ToString();
        }

        /// <summary>
        /// Dodaje zale¿ne assembly
        /// </summary>
        /// <param name="referencedAssembliesSet">Assembly do dodania.</param>
        /// <param name="assembly">Aktualnie rozpatrywany assembly.</param>
        private static void AddReferencedAssembly(HashSet<string> referencedAssembliesSet, Assembly assembly)
        {
            if (assembly.Location == "")
                assembly = Assembly.Load(assembly.FullName);

            if(!referencedAssembliesSet.Contains(assembly.Location))
            {
                referencedAssembliesSet.Add(assembly.Location);
                foreach (var asm in assembly.GetReferencedAssemblies().Select(x=> Assembly.Load(x)))
                    AddReferencedAssembly(referencedAssembliesSet, asm);
            }
        }

        private string BuildGenericName(Type type, HashSet<string> referencedAssembliesSet)
        {
            StringBuilder sb = new StringBuilder();
            string fullname = type.GetGenericTypeDefinition().FullName;
            sb.Append(fullname.Remove(fullname.IndexOf("`")));
            sb.Append("<");
            bool isFirst = true;
            foreach (Type t in type.GetGenericArguments())
            {
                referencedAssembliesSet.Add(t.Assembly.Location);
                if (!isFirst)
                    sb.Append(",");
                isFirst = false;
                if (t.IsGenericType)
                    sb.Append(BuildGenericName(t, referencedAssembliesSet));
                else
                    sb.Append(t.FullName);
            }
            sb.Append(">");
            return sb.ToString();
        }

        /// <summary>
        /// Zwraca posortowan¹ lisê domen aplikacji wed³ug kolejki.
        /// </summary>
        /// <param name="positions">Kolejka.</param>
        /// <returns>Lista AppDomain.</returns>
        private static AppDomainCacheWraper[] GetSortedAppDomainList(Dictionary<string, int> positions)
        {
            var appDomainList = cache.Select(x => x.Value.AppDomainCacheWraper).Distinct();
            var appDomainList2 = appDomainList.Select(x => new { AppDomainCacheWraper = x, Positions = x.CreatedScripts.Select(y => positions[y]) });
            return appDomainList2.OrderByDescending(x => x.Positions.Sum() / x.Positions.Count()).Select(x => x.AppDomainCacheWraper).ToArray();
        }
    }
}


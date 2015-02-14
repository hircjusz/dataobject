using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using Microsoft.VisualStudio.TextTemplating;

namespace SoftwareMind.Utils.Templating
{
    public class TemplatingHelper<EngineType, HostType> : IDisposable
        where EngineType : ITextTemplatingEngine, new()
        where HostType : ITextTemplatingEngineHost, new()
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TemplatingHelper<,>));

        private ITextTemplatingEngine _engine;
        private ITextTemplatingEngineHost _host;
        private IDictionary<string, object> _parameters;

        public void Init()
        {
            if (this._engine == null)
            {
                this._engine = new EngineType();
            }
            if (this._host == null)
            {
                this._host = new HostType();
            }
            if (this._parameters == null)
            {
                this._parameters = new Dictionary<string, object>();
            }
        }

        public void Dispose()
        {
            this._engine = null;
            this._host = null;
            this._parameters = null;
        }

        internal string LogParams(IDictionary<string, object> parameters)
        {
            StringBuilder result = new StringBuilder();

            if (this._parameters != null)
            {
                foreach (KeyValuePair<string, object> p in this._parameters)
                {
                    result.AppendFormat("\n\t{0} => {1}", p.Key, (!(p.Value is string) && p.Value is IEnumerable) ? string.Join(", ", ((IEnumerable)p.Value).Cast<object>()) : p.Value);
                }
            }
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> p in parameters)
                {
                    result.AppendFormat("\n\t{0} => {1}", p.Key, (!(p.Value is string) && p.Value is IEnumerable) ? string.Join(", ", ((IEnumerable)p.Value).Cast<object>()) : p.Value);
                }
            }

            return result.ToString();
        }

        internal string Transform(string input, IDictionary<string, object> parameters, string filePath = null)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "Input cannot be null");
            }

            this.Init();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    this._parameters.Add(parameter);
                }
            }

            if (this._host is ITextTemplatingEnhancedEngineHost)
            {
                ((ITextTemplatingEnhancedEngineHost) this._host).TemplateFile = filePath ?? Path.Combine(Environment.CurrentDirectory, "Dummy.tt");
            }

            if (this._parameters != null && this._parameters.Count > 0)
            {
                if (!(this._host is ITextTemplatingSessionHost))
                {
                    throw new InvalidOperationException("Specified host cannot access parameters");
                }

                ITextTemplatingSession session = ((ITextTemplatingSessionHost) this._host).Session;
                if (session == null)
                {
                    session = ((ITextTemplatingSessionHost) this._host).CreateSession();
                }
                if (session == null)
                {
                    throw new ArgumentNullException("Session cannot be null while specifing parameters");
                }

                foreach (var parameter in this._parameters)
                {
                    session[parameter.Key] = parameter.Value;
                }
            }

            string output = this._engine.ProcessTemplate(input, this._host);

            if (this._host is ITextTemplatingEnhancedEngineHost)
            {
                CompilerErrorCollection errors = ((ITextTemplatingEnhancedEngineHost) this._host).Errors;
                if (errors.Count > 0)
                {
                    StringBuilder errorString = new StringBuilder();
                    foreach (CompilerError error in errors)
                    {
                        errorString.AppendFormat("\n\t{0}:{1}:{2} - [{3}{4}] {5}", error.FileName, error.Line, error.Column, error.IsWarning ? "W" : "E", error.ErrorNumber, error.ErrorText);
                    }

                    log.ErrorFormat("Error occured while generating template{0}", errorString);
                }

                if (errors.HasErrors)
                {
                    var ex = new InvalidOperationException("Error occured while generating template");
                    ex.Data["Errors"] = errors;

                    throw ex;
                }
            }

            return output;
        }

        public string TransformFile(string filePath, IDictionary<string, object> parameters = null)
        {
            log.DebugFormat(
                "[T4] Transforming file {0} with params:{1}",
                filePath,
                this.LogParams(parameters)
            );

            string input = File.ReadAllText(filePath);

            return this.Transform(input, parameters, filePath);
        }

        public string TransformText(string input, IDictionary<string, object> parameters = null)
        {
            log.DebugFormat(
                "[T4] Transforming input {0} with params:{1}",
                input,
                this.LogParams(parameters)
            );

            return this.Transform(input, parameters);
        }

        public object this[string key]
        {
            get
            {
                this.Init();
                return this._parameters[key];
            }
            set
            {
                this.Init();
                this._parameters[key] = value;
            }
        }
    }

    public class DefaultTemplatingHelper : TemplatingHelper<Engine, DefaultTemplatingEngineHost>
    {
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using SoftwareMind.Shared.Config;
using log4net;

namespace SoftwareMind.Shared.Service
{
    [Export(typeof(ITextResourcesService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TextResourcesService : ITextResourcesService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TextResourcesService));

        private ITextResourcesServiceConfig _config;
        private string _resourceFile;
        private FileSystemWatcher _watcher;

        private ISet<string> _availableLanguages = new HashSet<string>();
        private IDictionary<string, string>  _languageMappings;
        private IDictionary<string, IDictionary<string, string>> _resources = new Dictionary<string, IDictionary<string, string>>();
        private ReadOnlyDictionary<string, IDictionary<string, string>> _publicResources = new ReadOnlyDictionary<string, IDictionary<string, string>>();
        private Action _fileChangedHandlerAction;
        private IAsyncResult _fileChangedHandlerResult;

        public event EventHandler Changed;

        [ImportingConstructor]
        public TextResourcesService(ITextResourcesServiceConfig config, IPathResolver pathResolver)
        {
            this._config = config;
            this._languageMappings = this._config.LanguageMapping.ToDictionary(e => e.Key, e => e.Value);
            this._resourceFile = pathResolver.ResolveResourcePath("resources.txt");
            this._fileChangedHandlerAction = () => { Thread.Sleep(100); this.Import(); };

            if (!File.Exists(this._resourceFile))
            {
                log.ErrorFormat("Resources directory is not configured or there is no such directory like specified one: \"{0}\"", this._resourceFile);
                throw new InvalidOperationException("Cannot find resource file");
            }

#if DEBUG
            this._watcher = new FileSystemWatcher(Path.GetDirectoryName(this._resourceFile));
            this._watcher.Changed += this.FileChangedHandler;
            this._watcher.Filter = Path.GetFileName(this._resourceFile);
            this._watcher.EnableRaisingEvents = true;
#endif

            this.Import();
        }

        ~TextResourcesService()
        {
#if DEBUG
            this._watcher.Changed -= this.FileChangedHandler;
            this._watcher.Dispose();
#endif
        }

        private void FileChangedHandler(object sender, FileSystemEventArgs e)
        {
            AsyncCallback callback = args => {
                lock (this)
                {
                    this._fileChangedHandlerResult = null;
                }
                if (this.Changed != null)
                {
                    this.Changed(this, EventArgs.Empty);
                }
            };

            lock (this)
            {
                if (this._fileChangedHandlerResult == null)
                {
                    this._fileChangedHandlerResult = this._fileChangedHandlerAction.BeginInvoke(callback, null);
                }
            }
        }

        private void Import()
        {
            using (TextReader reader = new StreamReader(File.OpenRead(this._resourceFile)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                    {
                        continue;
                    }

                    int separatorIndex = line.IndexOf('=');
                    int langIndex = line.LastIndexOf('-', separatorIndex);

                    string lang = line.Substring(langIndex + 1, separatorIndex - langIndex - 1);
                    string key = line.Substring(0, langIndex);
                    string value = line.Substring(separatorIndex + 1);

                    if (!this._languageMappings.ContainsKey(lang))
                    {
                        throw new InvalidOperationException("Invalid entry \"" + line + "\"");
                    }
                    lang = this._languageMappings[lang];

                    if (!this._availableLanguages.Contains(lang))
                    {
                        this._availableLanguages.Add(lang);
                        this._resources.Add(lang, new Dictionary<string, string>());
                        this._publicResources._dictionary.Add(lang, new ReadOnlyDictionary<string, string>(this._resources[lang]));
                    }

                    this._resources[lang][key] = value;
                }
            }
        }

        public IDictionary<string, string> GetEntries(string language)
        {
            if (!this._resources.ContainsKey(language))
            {
                throw new InvalidOperationException("Brak języka " + language);
            }
            return this._publicResources[language];
        }

        public IEnumerable<string> GetLanguages()
        {
            return this._availableLanguages.Select(l => l);
        }

        public IDictionary<string, string> GetLanguageMappings()
        {
            return this._languageMappings.ToDictionary(e => e.Key, e => e.Value);
        }

        public string GetText(string lang, string key)
        {
            if (!this._resources.ContainsKey(lang))
            {
                return this.InvalidLanguage(lang, key);
            }
            var entries = this._resources[lang];
            if (!entries.ContainsKey(key))
            {
                return this.InvalidKey(lang, key);
            }

            return entries[key];
        }

        public string GetTextFormatted(string lang, string key, params object[] parameters)
        {
            return string.Format(this.GetText(lang, key), parameters);
        }

        public string DefaultLanguage
        {
            get { return this._config.DefaultLanguage; }
        }

        private string InvalidKey(string lang, string key)
        {
            return string.Format("????{1}???? (Brak języka {0})", lang, key);
        }

        private string InvalidLanguage(string lang, string key)
        {
            return string.Format("????{1}????", lang, key);
        }
    }

    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        internal readonly IDictionary<TKey, TValue> _dictionary;

        public ReadOnlyDictionary()
        {
            this._dictionary = new Dictionary<TKey, TValue>();
        }

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this._dictionary = dictionary;
        }

        #region IDictionary<TKey,TValue> Members

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw ReadOnlyException();
        }

        public bool ContainsKey(TKey key)
        {
            return this._dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return this._dictionary.Keys; }
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw ReadOnlyException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this._dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return this._dictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return this._dictionary[key];
            }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return this[key];
            }
            set
            {
                throw ReadOnlyException();
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw ReadOnlyException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw ReadOnlyException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this._dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this._dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this._dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw ReadOnlyException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this._dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        private static Exception ReadOnlyException()
        {
            return new NotSupportedException("This dictionary is read-only");
        }
    }
}
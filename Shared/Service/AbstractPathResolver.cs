using System.Collections.Generic;
using System.IO;
using log4net;

namespace SoftwareMind.Shared.Service
{
    public abstract class AbstractPathResolver : IPathResolver
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AbstractPathResolver));

        protected abstract string ContentBasePath { get; }
        protected abstract string ResourcesBasePath { get; }
        protected abstract string TemporaryBasePath { get; }
        protected abstract string SystemTemporaryBasePath { get; }

        protected abstract IEnumerable<string> ContentAltetrnativePaths { get; }
        protected abstract IEnumerable<string> ResourcesAltetrnativePaths { get; }
        protected abstract IEnumerable<string> TemporaryAltetrnativePaths { get; }
        protected abstract IEnumerable<string> SystemTemporaryAltetrnativePaths { get; }

        protected virtual string ResolvePath(string relativePath, bool checkExistance, string basePath,
                                             IEnumerable<string> alternativePaths, string description)
        {
            _log.DebugFormat("Resolving server {0} path for \"{1}\"", description, relativePath);

            if (string.IsNullOrEmpty(relativePath))
            {
                _log.DebugFormat("Resolving server {0} path for \"{1}\": path is empty, result is \"{2}\"", description,
                                 relativePath, basePath);
                return basePath;
            }

            if (Path.IsPathRooted(relativePath))
            {
                _log.DebugFormat("Resolving server {0} path for \"{1}\": path is absolute", description, relativePath);
                return relativePath;
            }

            if (basePath != null)
            {
                string candidate = Path.Combine(basePath, relativePath);
                if (!checkExistance || File.Exists(candidate) || Directory.Exists(candidate))
                {
                    _log.DebugFormat("Resolving server {0} path for \"{1}\": result is \"{2}\"", description,
                                     relativePath, candidate);
                    return candidate;
                }
            }

            if (alternativePaths != null)
            {
                foreach (var alternativePath in alternativePaths)
                {
                    string candidate = Path.Combine(alternativePath, relativePath);
                    if (File.Exists(candidate) || Directory.Exists(candidate))
                    {
                        _log.DebugFormat("Resolving server {0} path for \"{1}\": result is \"{2}\"", description,
                                         relativePath, candidate);
                        return candidate;
                    }
                }
            }


            return null;
        }

        public virtual string ResolveContentPath(string relativePath = null, bool checkExistance = true)
        {
            return this.ResolvePath(relativePath, checkExistance, this.ContentBasePath, this.ContentAltetrnativePaths, "content");
        }

        public virtual string ResolveResourcePath(string relativePath = null, bool checkExistance = true)
        {
            return this.ResolvePath(relativePath, checkExistance, this.ResourcesBasePath, this.ResourcesAltetrnativePaths, "resource");
        }

        public virtual string ResolveTemporaryPath(string relativePath = null, bool checkExistance = true)
        {
            return this.ResolvePath(relativePath, checkExistance, this.TemporaryBasePath, this.TemporaryAltetrnativePaths, "temporary");
        }

        public virtual string ResolveSystemTemporaryPath(string relativePath = null, bool checkExistance = true)
        {
            return this.ResolvePath(relativePath, checkExistance, this.SystemTemporaryBasePath, this.SystemTemporaryAltetrnativePaths, "system temporary");
        }
    }
}

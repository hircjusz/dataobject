using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace SoftwareMind.Utils.Exceptions
{
    [Serializable]
    public abstract class TranslatedApplicationException : ApplicationException
    {
        private const int RESULT_CODE = 1001; // TODO: remove duplication
        private const bool SUCCESS = false;

        [SecuritySafeCritical]
        protected TranslatedApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public TranslatedApplicationException(ExceptionCategory category)
            : this(category, true) { }

        public TranslatedApplicationException(ExceptionCategory category, string message)
            : this(category, true, message) { }

        public TranslatedApplicationException(ExceptionCategory category, string message, Exception innerException)
            : this(category, true, message, innerException) { }

        public TranslatedApplicationException(ExceptionCategory category, bool translated)
            : base()
        {
            this.Init(category, translated);
        }

        public TranslatedApplicationException(ExceptionCategory category, bool translated, string message)
            : base(message)
        {
            this.Init(category, translated);
        }

        public TranslatedApplicationException(ExceptionCategory category, bool translated, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Init(category, translated);
        }

        private void Init(ExceptionCategory category, bool translated)
        {
            this.Category = category;
            this.Translated = translated;

            this.Data["errorMessage"] = this.Message;
            this.Data["resultCode"] = RESULT_CODE;
            this.Data["success"] = SUCCESS;
        }

        public ExceptionCategory Category
        {
            get
            {
                ExceptionCategory result = ExceptionCategory.ERROR;
                if (this.Data.Contains("category"))
                {
                    var value = Enum.GetValues(typeof(ExceptionCategory)).Cast<ExceptionCategory>().Where(x => x.ToString().Equals(this.Data["category"])).ToArray();
                    if (value.Length == 1)
                    {
                        result = value.First();
                    }
                }

                return result;
            }
            set
            {
                this.Data["category"] = value.ToString();
            }
        }

        public bool Translated
        {
            get
            {
                bool result = true;

                if (this.Data.Contains("translated"))
                {
                    try
                    {
                        result = Convert.ToBoolean(this.Data["translated"]);
                    }
                    catch { }
                }

                return result;
            }
            set
            {
                this.Data["translated"] = value;
            }
        }

        public bool IsDefaultMessageVisible
        {
            get
            {
                bool result = true;

                if (this.Data.Contains("isDefaultMessageVisible"))
                {
                    try
                    {
                        result = Convert.ToBoolean(this.Data["isDefaultMessageVisible"]);
                    }
                    catch { }
                }

                return result;
            }
            set
            {
                this.Data["isDefaultMessageVisible"] = value;
            }
        }
    }
}

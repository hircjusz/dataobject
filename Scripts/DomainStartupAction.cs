using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.Scripts
{
    /// <summary>
    /// W skryptach często są wywoływane akcje z logiki biznesowej które mogą używać różnych zależności
    /// np. IOC. W związku z tym że skrypty są wykonywane w innych domenach aplikacji, potrzeba podczas budowy
    /// domeny wywoływać akcje inicjalizacyjne.
    ///
    /// Ta klasa jest po to by ustawiać zmienną StartupCode która powinna być zainicjalizowana w każdej domenie
    /// i powinna być przekazywana w dół.
    /// </summary>
    public class DomainStartupAction : MarshalByRefObject
    {
        protected static Action StartupCodeAction;

        public static Action StartupCode
        {
            get
            {
                return StartupCodeAction;
            }
            set
            {
                if (StartupCodeAction != null)
                {
                    throw new InvalidOperationException("StartupCode można ustawić tylko raz");
                }
                StartupCodeAction = value;
            }
        }

        public void SetStartupScript(Action action)
        {
            DomainStartupAction.StartupCode = action;

            if (action != null)
            {
                action();
            }
        }
    }

}

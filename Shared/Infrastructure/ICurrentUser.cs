using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.Shared.Infrastructure
{
    public interface ICurrentUser
    {
        /// <summary>
        /// język aplikacji, w jakim pracuje użytkownik (jeśli nie ustawiono inaczej, to jest to
        /// domyślny język ustawiony w pliku externalAppSettings.config, klucz: TextResources.DefaultLanguage)
        /// </summary>
        string UserLanguage { get; }

        long ProfileID { get; }

        string ProfileKind { get; }

        string UserProfile { get; }

        string AccessRightsBusinessLine { get; }

        string AccessRightsDepartment { get; }

        string AccessRightsC5 { get; }

        /// <summary>
        /// uprawnienia do mnemoników
        /// </summary>
        string AccessRightsMnemonic { get; }

        /// <summary>
        /// uprawnienia do mnemoników analityków
        /// </summary>
        string AccessRightsAnalystMnemonic { get; }

        /// <summary>
        /// uprawnienia do rodzajów procesu
        /// </summary>
        string AccessRightsProcessKind { get; }

        string AccessRightsProduct { get; }

        /// <summary>
        /// uprawnienia do zaangażowania na kliencie
        /// </summary>
        string AccessRightsLOO { get; }

        /// <summary>
        /// uprawnienia do zaangażowania na grupie, do której należy klient
        /// w szczególnym przypadku może to być grupa składająca się tylko z jednego klienta
        /// </summary>
        string AccessRightsLOOGroup { get; }

        string AccessRightsGRM { get; }

        string AccessRightsProfileCode { get; }

        IEnumerable<string> AccessRightsCompetences { get; }

        bool AccessRightsGRMOperator { get; }

        // Venus
        string OrgUnitsTaskPrivileges { get; }
        IEnumerable<string> AccessRightsCaseSegment { get; }

        // Task Visibility
        bool AccessRightsDirectlyConnectedOrgUnitTV { get; }
        bool AccessRightsFellowEmployeesTV { get; }
        bool AccessRightsSubOrgUnitsTV { get; }
        bool AccessRightsSuperUnitsTV { get; }
        bool AccessRightsFullTV { get; }

        String UserName { get; }

        /// <summary>
        /// Adres email zalogowanego użytkownika
        /// </summary>
        String Email { get; }

        String DomainName { get; }

        long EffectiveUserId { get; }

        long LoggedUserId { get; }

        string LoggedUserName { get; }

        /// <summary>
        /// nazwa użytkownika/zajmowane stanowisko
        /// przykład: lupo/RM - użytkownik, na którego jestem zalogowany to lupo, stanowisko to RM
        /// uwaga: w przypadku, gdy zastępujemy kogoś, to wyświetlamy tutaj osobę, na którą się logujemy i jej stanowisko
        /// na przykład: mana/Admin, chociaż moja nazwa użytkownika to lupo
        /// </summary>
        string UserPosition { get; }

        /// <summary>
        /// unikalne ID sesji HTTP (nie mylić z sesją DataObjects - sesja DataObjects budowana jest na podstawie sesji HTTP)
        /// </summary>
        string SessionID { get; }

        //string ProfileCode { get; }

        IEnumerable<string> LoggedUserProfileCodes { get; }

        bool Rewritten { get; }

        /// <summary>
        /// Application type choosen by user
        /// </summary>
        string UserApplication { get; }

        /// <summary>
        /// Pełna nazwa profilu
        /// </summary>
        string UserProfileName { get; }

        /// <summary>
        /// Brak uprawnień do kanału MARS
        /// </summary>
        bool MarsAccessDenied { get; }

        /// <summary>
        /// Brak uprawnień do kanału WENUS
        /// </summary>
        bool WenusAccessDenied { get; }

    }
}

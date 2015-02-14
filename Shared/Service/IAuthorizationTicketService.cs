using SoftwareMind.Shared.Dto;
using SoftwareMind.Shared.Infrastructure;

namespace SoftwareMind.Shared.Service
{
    public interface IAuthorizationTicketService : IService
    {
        IContainer GetAuthorizationTicket<T>(AccessStoreActionDTO<T> action, long documentId);
        IContainer UpdateAuthorizationTicket<T>(ModifyStoreActionDTO<T> action, long documentId);

        IContainer GetLoanAdditionalInfo<T>(AccessStoreActionDTO<T> action, long authorizationTicketId);

        IContainer GetProducts<T>(AccessStoreActionDTO<T> action, long authorizationTicketId);
        IContainer UpdateProducts<T>(ModifyStoreActionDTO<T> action);
        IContainer DeleteProducts<T>(DeleteStoreActionDTO<T> action, long authorizationTicketId);
        IContainer GetNestedProducts<T>(AccessStoreActionDTO<T> action, long authorizationTicketId, long parentProductId);

        IContainer GetApplicationDeciAccepts<T>(AccessStoreActionDTO<T> action, long processId, long applicationId, long? procTaskId);

        IContainer GetBorrowers<T>(AccessStoreActionDTO<T> action, long productId);
        IContainer GetRatings<T>(AccessStoreActionDTO<T> action, long authorizationTicketId);
        IContainer GetSuppliants<T>(AccessStoreActionDTO<T> action, long authorizationTicketId, string roleFilter, long? suppliantId);
        IContainer GetSuppliant<T>(AccessStoreActionDTO<T> action, long authorizationTicketId, bool filterNonCustomers, string number, string pesel, string regon,string cardid, string name);
        IContainer UpdateSuppliants<T>(ModifyStoreActionDTO<T> action, long collateralId);
        IContainer DeleteSuppliants<T>(DeleteStoreActionDTO<T> action);

        IContainer GetMonitoringDocs<T>(AccessStoreActionDTO<T> action, long processId);
        IContainer GetProductAdditionalCondition<T>(AccessStoreActionDTO<T> action, long productId);
        IContainer GetProductDistributeCondition<T>(AccessStoreActionDTO<T> action, long productId);
        void SetBorrowersToNewProduct(long productId, long[] borrowerIds);
        bool RemoveBorrowersForNewProduct(long productId, long[] borrowerIds);
        bool CheckCanAddAndEditProduct(long documentId);
        IContainer GetProductPolicyAsCollInfo<T>(AccessStoreActionDTO<T> action, long productId);

        IContainer GetProductCollateral<T>(AccessStoreActionDTO<T> action, long productId);
        IContainer UpdateProductCollateral<T>(ModifyStoreActionDTO<T> action);
        IContainer DeleteProductCollateral<T>(DeleteStoreActionDTO<T> action);

        IContainer GetProductJournals(AccessStoreActionDTO<object> action, long productId);
        IContainer GetDecisionJournals(AccessStoreActionDTO<object> action, long decisionId);
        void UpdateDocumentLanguage(long documentId, string language);
        void RefreshCustomerMarsProducts(long documentId, long customerId);
        void RefreshAuthorizationTicketMarsProducts(long documentId);

        IContainer GenerateWordDocument(long documentId);
        string DeleteWordDocument(long fileId);
        string AttachWordDocument(long documentId, long fileId);
        IContainer GetProcessRolesWithAssigneeForProcess<T>(AccessStoreActionDTO<T> action, long processId);
        bool RestoreProduct(long productId);
        object CheckRatingsFillingIsRequired(long processId);

        IContainer AddMonitoringDocument(long productId, long period, string condition, string comment);
        IContainer AddMonitoringDocuments(long productId, long[] periods, string[] conditions, string[] comments);
        IContainer AddDistributeCondition(long productId, long category, string condition, string comment);
        IContainer AddDistributeConditions(long productId, long[] categories, string[] conditions, string[] comments);
        IContainer AddAdditionalCondition(long productId, long category, string condition, string comment);
        IContainer AddAdditionalConditions(long productId, long[] categories, string[] conditions, string[] comments);

        IContainer CopyCollateral(long collateralId, long productId, bool isReference);
        IContainer CopyCollaterals(long[] collateralId, long productId, bool isReference);
        IContainer RemoveCollateral(long collateralId, long productId, long? newMasterCollateralId);

        IContainer GetQualifiers<T>(AccessStoreActionDTO<T> action, long authorizationTicketId, long processId);
        IContainer GetQualifiersOwners<T>(AccessStoreActionDTO<T> action, long authorizationTicketId, long processId);

        IContainer UpdateQualifiers<T>(ModifyStoreActionDTO<T> action);

        IContainer CopyAuthorizationTicketData(long sourceId, long targetId);

        IContainer CreateProcessFromAuthorizationTicket(long processId, long? authorizationTicketId, long defNumber, string segment);

        IContainer GetOtherProducts<T>(AccessStoreActionDTO<T> action, long authorizationTicketId, long excludedProductId);
    }
}

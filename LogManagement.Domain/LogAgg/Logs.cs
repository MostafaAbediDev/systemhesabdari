using _0_FrameWork.Domain;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using PersonManagement.Domain.Person.PersonAgg;

namespace LogManagement.Domain.LogAgg
{
    public class Logs : EntityBase
    {
        public string Module { get; private set; }
        public string Action { get; private set; }
        public string ActionType { get; private set; }
        public string EntityName { get; private set; }
        public string OldValue { get; private set; }
        public string NewValue { get; private set; }
        public string Severity { get; private set; }
        public string Status { get; private set; }
        public string ErrorMessage { get; private set; }
        public string StackTrace { get; private set; }
        public string IpAddress { get; private set; }
        public string UserAgent { get; private set; }
        public DateTime? ClientDateTime { get; private set; }
        public int ExecutionTimeMs { get; private set; }
        public long EntityId { get; private set; }
        public long BranchId { get; private set; }
        public long UserId { get; private set; }
        public Branches Branches { get; private set; }
        public Persons Persons { get; private set; }

        public Logs(string module, string action, string actionType, string entityName, string oldValue, 
            string newValue, string severity, string status, string errorMessage, string stackTrace, 
            string ipAddress, string userAgent, DateTime? clientDateTime, int executionTimeMs, long entityId)
        {
            Module = module;
            Action = action;
            ActionType = actionType;
            EntityName = entityName;
            OldValue = oldValue;
            NewValue = newValue;
            Severity = severity;
            Status = status;
            ErrorMessage = errorMessage;
            StackTrace = stackTrace;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            ClientDateTime = clientDateTime;
            ExecutionTimeMs = executionTimeMs;
            EntityId = entityId;
        }

        public void Edit(string module, string action, string actionType, string entityName, string oldValue,
            string newValue, string severity, string status, string errorMessage, string stackTrace,
            string ipAddress, string userAgent, DateTime? clientDateTime, int executionTimeMs, long entityId)
        {
            Module = module;
            Action = action;
            ActionType = actionType;
            EntityName = entityName;
            OldValue = oldValue;
            NewValue = newValue;
            Severity = severity;
            Status = status;
            ErrorMessage = errorMessage;
            StackTrace = stackTrace;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            ClientDateTime = clientDateTime;
            ExecutionTimeMs = executionTimeMs;
            EntityId = entityId;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void Active()
        {
            IsActive = true;
        }

        public void NotActive()
        {
            IsActive = false;
        }
    }
}

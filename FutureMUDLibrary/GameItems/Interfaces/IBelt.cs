using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces {
    public enum IBeltCanAttachBeltableResult {
        Success,
        FailureTooLarge,
        FailureExceedMaximumNumber,
        NotValidType
    }

    public interface IBelt : IGameItemComponent {
        SizeCategory MaximumSize { get; }
        int MaximumNumberOfBeltedItems { get; }
        IEnumerable<IBeltable> ConnectedItems { get; }
        void AddConnectedItem(IBeltable item);
        void RemoveConnectedItem(IBeltable item);
        IBeltCanAttachBeltableResult CanAttachBeltable(IBeltable beltable);
    }
}
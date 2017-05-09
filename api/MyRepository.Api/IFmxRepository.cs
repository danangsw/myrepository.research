using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRepository.Api
{
    public interface IFmxRepository
    {
        // Store an item to the repository.
        // Parameter itemType is used to differentiate JSON or XML.
        // 1 = itemContent is a JSON string.
        // 2 = itemContent is an XML string.
        void Register(string itemName, string itemContent, int itemType);

        // Retrieve an item from the repository.
        string Retrieve(string itemName);

        // Retrieve the type of the item (JSON or XML).
        int GetType(string itemName);

        // Remove an item from the repository.
        void Deregister(string itemName);

        // Initialize the repository for use, if needed.
        // You could leave it empty if you have your own way to make the repository ready for use
        // (e.g. using the constructor).
        void Initialize(); 
    }
}

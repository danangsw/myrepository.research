using FluentValidation.Results;
using MyRepository.Ninject;
using MyRepository.Serializers;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRepository.Api
{
    public class FmxRepository : IFmxRepository
    {
        StandardKernel kernel = null;
        Repository repository;

        public void Register(string itemName, string itemContent, int itemType)
        {
            try
            {
                ValidateInput(itemContent, itemType);

                if (repository == null)
                {
                    throw new NullReferenceException("FmxRepository has not been initialized.");
                }

                repository.Update<ApiEntity>(itemName, context =>
                {
                    context.Entity.ItemContent = itemContent;
                    context.Entity.ItemType = itemType;
                });
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (ImmutableObjectException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public string Retrieve(string itemName)
        {
            if (repository == null)
            {
                throw new NullReferenceException("FmxRepository has not been initialized.");
            }

            var entity = repository.Read<ApiEntity>(itemName);

            if (entity == null)
            {
                throw new NullReferenceException(String.Format("Entity for {0} is not found.", itemName));
            }

            return entity.ItemContent;
        }

        public int GetType(string itemName)
        {
            if (repository == null)
            {
                throw new NullReferenceException("FmxRepository has not been initialized.");
            }

            var entity = repository.Read<ApiEntity>(itemName);

            if (entity == null)
            {
                throw new NullReferenceException(String.Format("Entity for {0} is not found.", itemName));
            }

            return entity.ItemType;
        }

        public void Deregister(string itemName)
        {
            if (repository == null)
            {
                throw new NullReferenceException("FmxRepository has not been initialized.");
            }

            repository.Remove<ApiEntity>(itemName);
        }

        public void Initialize()
        {
            if (kernel == null)
            {
                kernel = new StandardKernel();
            }
            else
            {
                throw new InvalidOperationException("FmxRepository has been initialized. Just called this method once at the first time.");
            }

            var builder = new NinjectObjectBuilder(kernel);
            RepositoryConfigure.SetObjectBuilder(builder);
            RepositoryConfigure.SetObjectConfigurator(builder);

            RepositoryConfigure
                .Persist<ApiEntity>()
                .Immutable()
                .WithTypeAlias("api_entity")
                .MemoryRepository(new BinarySerializer());

            repository = kernel.Get<Repository>();
        }

        private void ValidateInput(string itemContent, int itemType)
        {
            ApiEntity entity = new ApiEntity() { ItemType = itemType, ItemContent = itemContent};
            ApiEntityValidator validator = new ApiEntityValidator();

            ValidationResult results = validator.Validate(entity);

            if (!results.IsValid)
            {
                var errors = results.Errors;
                List<string> errorList = new List<string>();

                foreach (var item in errors)
                {
                    errorList.Add(item.ErrorMessage); 
                }

                throw new InvalidOperationException(String.Join(", ",errorList));
            }
        }
    }
}

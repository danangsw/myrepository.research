using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MyRepository.Api
{
    [Validator(typeof(ApiEntityValidator))]
    [Serializable]
    public class ApiEntity
    {
        public string ItemContent { get; set; }
        
        /// <summary>
        /// 1:JSON string, 2:XML string
        /// </summary>
        public int ItemType { get; set; }

        public override string ToString()
        {
            return string.Format("ItemType:{0}, ItemContent: {1}", ItemType, ItemContent);
        }
    }

    public class ApiEntityValidator : AbstractValidator<ApiEntity>
    {
        public ApiEntityValidator()
        {
            RuleFor(x => x.ItemContent).NotEmpty().WithMessage("Item content cannot be empty.");
            RuleFor(x => x.ItemType).GreaterThan(0).WithMessage("Fill item type with following types, 1:JSON or 2:XML")
                                    .LessThan(3).WithMessage("Fill item type with following types, 1:JSON or 2:XML");

            RuleFor(x => x.ItemContent)
                 .Must(ValidateJSON) // XML validation
                 .When(x => x.ItemType == 1)
                 .WithMessage("Invalid JSON format");

            RuleFor(x => x.ItemContent)
                 .Must(ValidateXML) // XML validation
                 .When(x => x.ItemType == 2)
                 .WithMessage("Invalid XML format");
        }

        private bool ValidateJSON(string input)
        {
            input = input.Trim();
            if ((input.StartsWith("{") && input.EndsWith("}")) || //For object
                (input.StartsWith("[") && input.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(input);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool ValidateXML(string xmlString)
        {
            Regex tagsWithData = new Regex("<\\w+>[^<]+</\\w+>");

            //Light checking
            if (string.IsNullOrEmpty(xmlString) || tagsWithData.IsMatch(xmlString) == false)
            {
                return false;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace XCloud.GeoLocation.Filters
{
    public class ValidateModalAttribute : ActionFilterAttribute
    {
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.ModelState.IsValid)
			{
				context.Result = new ValidationFailedResult(context.ModelState);
			}
		}

		public class ValidationFailedResult : ObjectResult
		{
			public ValidationFailedResult(ModelStateDictionary modelState)
				: base(new ValidationResultModel(modelState))
			{
				StatusCode = StatusCodes.Status422UnprocessableEntity;
			}
		}

		public class ValidationError
		{
			public ValidationError(string field, string message)
			{
				Field = field != string.Empty ? field : null;
				Message = message;
			}

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string Field { get; }

			public string Message { get; }
		}

		public class ValidationResultModel
		{
			public ValidationResultModel(ModelStateDictionary modelState)
			{
				Message = "Validation Failed";
				Errors = modelState.Keys
						.SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
						.ToList();
			}

			public string Message { get; }

			public List<ValidationError> Errors { get; }
		}
	}
}

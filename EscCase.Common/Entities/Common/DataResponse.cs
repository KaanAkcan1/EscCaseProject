namespace EscCase.Common.Entities.Common
{
    public class DataResponse<Tkey>
    {
        public string Message { get; set; } = string.Empty;

        public bool Success { get; set; } = false;

        public int StatusCode { get; set; } = 500;

        public IEnumerable<ApiError> Error { get; set; } = new List<ApiError>().AsEnumerable();

        public Tkey? Data { get; set; }

        public void SetError(string message, int? statusCode = 500)
        {
            Success = false;
            StatusCode = statusCode ?? 500;
            Message = message;
        }
        public void SetRecordCouldNotFound(string? message = "")
        {
            SetError("Record not found !" + message == "" ? "" : (" // " + message));
        }

        public void SetRecordFounded(Tkey? data)
        {
            SetSuccess(data, "Record(s) retrieved successfully");
        }


        public void SetSuccess(Tkey? data, string message = "", int? statusCode = 200)
        {
            Success = true;
            StatusCode = statusCode ?? 200;
            Message = Message == string.Empty ? message : Message + " - " + message;
            Data = data;
        }

        public void SetSuccessList(Tkey? data, string message = "", int? statusCode = 200)
        {
            SetSuccess(data, "The list has been successfully retrieved from the database");
        }

        public void SetSuccesCreate(Tkey? data, string message = "", int? statusCode = 200)
        {
            Success = true;
            StatusCode = statusCode ?? 200;
            Message = "Record created successfully";
            Data = data;
        }

        public void SetErrorCreate(string message = "", int? statusCode = 500)
        {
            Success = false;
            StatusCode = statusCode ?? 500;
            Message = "Record could not be created => " + message;
        }
    }
}

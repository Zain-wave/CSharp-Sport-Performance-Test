namespace SportsSebastianVargas.Response;

public class ResponseService<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }

    public ResponseService(T? data, string? message = null, bool success = true)
    {
        Data = data;
        Message = message;
        Success = success;
    }
}
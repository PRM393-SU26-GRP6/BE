namespace CourtManager.Application.DTOs;

public record FileUploadDto(Stream Content, string FileName, string ContentType);

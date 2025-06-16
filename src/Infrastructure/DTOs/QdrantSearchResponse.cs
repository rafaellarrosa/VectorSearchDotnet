using System.Collections.Generic;

namespace Infrastructure.DTOs;

public class QdrantSearchResponseDto
{
    public List<SearchResultDto> Result { get; set; }
    public string Status { get; set; }
    public double Time { get; set; }
}

public class SearchResultDto
{
    public string Id { get; set; }
    public float Score { get; set; }
    public QdrantPayloadDto? Payload { get; set; }
}
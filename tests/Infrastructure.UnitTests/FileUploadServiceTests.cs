using FluentAssertions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.UnitTests;

public class FileUploadServiceTests
{
    private readonly Mock<ILogger<FileUploadService>> _loggerMock;
    private readonly FileUploadService _service;

    public FileUploadServiceTests()
    {
        _loggerMock = new Mock<ILogger<FileUploadService>>();
        _service = new FileUploadService(_loggerMock.Object);
    }

    #region UploadImageAsync Tests

    [Fact]
    public async Task UploadImageAsync_ShouldReturnUniqueFileName_WhenFileIsValid()
    {
        // Arrange
        var file = CreateMockFormFileWithStream("test.jpg", 100, "image/jpeg");

        // Act
        var result = await _service.UploadImageAsync(file.Object);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().EndWith(".jpg");
        Guid.TryParse(Path.GetFileNameWithoutExtension(result), out _).Should().BeTrue();
    }

    [Fact]
    public async Task UploadImageAsync_ShouldThrow_WhenFileIsInvalid()
    {
        // Arrange
        var file = CreateMockFormFile("test.exe", 1024, "application/octet-stream");

        // Act
        var act = async () => await _service.UploadImageAsync(file.Object);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid file type or size");
    }

    #endregion

    #region Helper Methods

    private static Mock<IFormFile> CreateMockFormFile(string fileName, long length, string contentType)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(length);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        return fileMock;
    }

    private static Mock<IFormFile> CreateMockFormFileWithStream(string fileName, int contentLength, string contentType)
    {
        var content = new byte[contentLength];
        new Random().NextBytes(content);
        var stream = new MemoryStream(content);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(contentLength);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>((targetStream, token) =>
            {
                stream.Position = 0;
                return stream.CopyToAsync(targetStream, token);
            });

        return fileMock;
    }

    #endregion
}

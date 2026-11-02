using EduHub.Application.Clubs;
using EduHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EduHub.API.Controllers;

/// <summary>Quản lý câu lạc bộ. Controller chỉ nhận request, gọi handler của Application rồi trả status code.</summary>
[ApiController]
[Route("api/clubs")]
public class ClubsController : ControllerBase
{
    private readonly GetClubsQueryHandler _getClubsHandler;
    private readonly GetClubByIdQueryHandler _getClubByIdHandler;
    private readonly CreateClubCommandHandler _createClubHandler;
    private readonly DeleteClubCommandHandler _deleteClubHandler;

    public ClubsController(
        GetClubsQueryHandler getClubsHandler,
        GetClubByIdQueryHandler getClubByIdHandler,
        CreateClubCommandHandler createClubHandler,
        DeleteClubCommandHandler deleteClubHandler)
    {
        _getClubsHandler = getClubsHandler;
        _getClubByIdHandler = getClubByIdHandler;
        _createClubHandler = createClubHandler;
        _deleteClubHandler = deleteClubHandler;
    }

    // Các method dưới đây chưa có async để starter build được không cảnh báo.
    // Khi viết thân method, hãy thêm từ khoá async vào chữ ký.

    /// <summary>Lấy danh sách câu lạc bộ.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Club>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        // TODO 8a: gọi _getClubsHandler.HandleAsync(new GetClubsQuery(), ct)
        var clubs = await _getClubsHandler.HandleAsync(new GetClubsQuery(), ct);
        // TODO 8b: trả Ok(clubs) → 200
        return Ok(clubs);
    }

    /// <summary>Lấy một câu lạc bộ theo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Club), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        // TODO 8c: gọi _getClubByIdHandler.HandleAsync(new GetClubByIdQuery(id), ct)
        var club = await _getClubByIdHandler.HandleAsync(new GetClubByIdQuery(id), ct);
        // TODO 8d: không có club → NotFound() (404); có → Ok(club) (200)
        return club is null ? NotFound() : Ok(club);
    }

    /// <summary>Tạo câu lạc bộ mới.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Club), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateClubRequest request, CancellationToken ct)
    {
        try {
            // TODO 8e: đổi request thành CreateClubCommand, gọi _createClubHandler.HandleAsync(command, ct)
            // TODO 8f: thành công -> CreatedAtAction(nameof(GetById), new { id = club.Id }, club) (201 + Location)
            var command = new CreateClubCommand(request.Name, request.Description);
            var club = await _createClubHandler.HandleAsync(command, ct);
            return CreatedAtAction(nameof(GetById), new { id = club.Id }, club);
        }
        catch (ArgumentException ex) {
            // TODO 8g: bắt ArgumentException -> BadRequest(new { message = ex.Message }) (400)
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Xoá một câu lạc bộ.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try {
            // TODO 8h: gọi _deleteClubHandler.HandleAsync(new DeleteClubCommand(id), ct) -> NoContent() (204)
            await _deleteClubHandler.HandleAsync(new DeleteClubCommand(id), ct);
            return NoContent();
        }
        catch (KeyNotFoundException) {
            // TODO 8i: bắt KeyNotFoundException -> NotFound() (404)
            return NotFound();
        }
    }
}

/// <summary>Dữ liệu client gửi lên khi tạo CLB (DTO/mapping đầy đủ sẽ học ở Chương 4).</summary>
public record CreateClubRequest(string Name, string Description);

// Giải thích chi tiết:
// GetAll (TODO 8a, 8b): Gọi `GetClubsQueryHandler` để lấy toàn bộ danh sách và bọc trong `Ok(clubs)` để trả về mã HTTP `200`.
// GetById (TODO 8c, 8d): Truy vấn theo ID, nếu kết quả trả về là `null` thì trả về `404 NotFound`, ngược lại trả về `200 OK` kèm theo dữ liệu.
// Create (TODO 8e, 8f, 8g): Nhận request, chuyển đổi thành lệnh `CreateClubCommand`, gọi handler để xử lý và trả về `201 Created` kèm theo đường dẫn `Location` của tài nguyên mới. Nếu dữ liệu không hợp lệ (bắt ngoại lệ `ArgumentException`), trả về `400 Bad Request`.
// Delete (TODO 8h, 8i): Thực hiện lệnh xóa, nếu thành công trả về `204 No Content`. Nếu không tìm thấy ID cần xóa (bắt ngoại lệ `KeyNotFoundException`), trả về `404 Not Found`.

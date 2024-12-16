using MatajuApi.Models;
using MatajuApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatajuApi.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] TODO: 활성화 및 Role 확인
public class AdminController : ControllerBase
{
    private readonly IRepository<House> _houseTableRepo;
    private readonly IRepository<Unit> _unitRepo;
    private readonly IRepository<Booking> _bookingRepo;


    /// <summary>
    /// 생성자 - Program DI Container로부터 레포지토리 주입
    /// </summary>
    /// <param name="houseTableRepo">창고 지점 테이블 레포지토리</param>
    /// <param name="unitRepo">유닛 테이블 레포지토리</param>
    /// <param name="bookingRepo">예약 테이블 레포지토리</param>
    public AdminController(IRepository<House> houseTableRepo, IRepository<Unit> unitRepo, IRepository<Booking> bookingRepo)
    {
        _houseTableRepo = houseTableRepo;
        _unitRepo = unitRepo;
        _bookingRepo = bookingRepo;
    }

    /// <summary>
    /// House 레포지토리에 임의의 데이터 추가 (개발용 임시 엔드포인트).
    /// </summary>
    /// <returns>추가된 House 데이터 목록</returns>
    [HttpPost("seed-houses")]
    public IActionResult SeedHouses()
    {
        // 임의 데이터 생성
        var sampleHouses = new List<House>
                           {
                               new House { Add = "서울특별시 종로구 세종로 1번지", Province = "서울특별시", PriceS = 35000, PriceM = 78000, PriceL = 119000 },
                               new House { Add = "부산광역시 해운대구 해운대로 620번길 15", Province = "부산광역시", PriceS = 24000, PriceM = 36000, PriceL = 69000 },
                               new House { Add = "대구광역시 수성구 동대구로 85번길 12", Province = "대구광역시", PriceS = 24000, PriceM = 36000, PriceL = 69000 },
                               new House { Add = "인천광역시 연수구 송도동 23-1", Province = "인천광역시", PriceS = 23000, PriceM = 32000, PriceL = 60000 },
                               new House { Add = "광주광역시 북구 우치로 200번길 5", Province = "광주광역시", PriceS = 23000, PriceM = 32000, PriceL = 60000 },
                               new House { Add = "대전광역시 서구 둔산대로 100번길 10", Province = "대전광역시", PriceS = 24000, PriceM = 36000, PriceL = 69000 },
                               new House { Add = "울산광역시 남구 삼산로 95번길 20", Province = "울산광역시", PriceS = 23000, PriceM = 32000, PriceL = 60000 },
                               new House { Add = "경기도 수원시 영통구 매탄로 33번길 18", Province = "경기도", PriceS = 23000, PriceM = 32000, PriceL = 60000 },
                               new House { Add = "강원도 춘천시 봉의동 중앙로 50번길 7", Province = "강원도", PriceS = 21000, PriceM = 28000, PriceL = 45000 },
                               new House { Add = "충청북도 청주시 상당구 서원로 17번길 5", Province = "충청북도", PriceS = 21000, PriceM = 28000, PriceL = 45000 },
                               new House { Add = "충청남도 천안시 서북구 백석로 25번길 9", Province = "충청남도", PriceS = 35000, PriceM = 78000, PriceL = 119000 },
                               new House { Add = "전라북도 전주시 완산구 효자동3가 23-10", Province = "전라북도", PriceS = 21000, PriceM = 28000, PriceL = 45000 },
                               new House { Add = "전라남도 여수시 학동 20번길 8", Province = "전라남도", PriceS = 21000, PriceM = 28000, PriceL = 45000 },
                               new House { Add = "경상북도 포항시 북구 양덕로 12번길 11", Province = "경상북도", PriceS = 21000, PriceM = 28000, PriceL = 45000 },
                               new House { Add = "경상남도 창원시 성산구 중앙대로 30번길 15", Province = "경상남도", PriceS = 21000, PriceM = 28000, PriceL = 45000 }
                           };

        _houseTableRepo.SetInitialData(sampleHouses);

        return Ok(new
                  {
                      Message = $"{sampleHouses.Count}개의 House 데이터가 추가되었구유.",
                      Houses = sampleHouses
                  });
    }

    /// <summary>
    /// Unit 레포지토리에 임의의 데이터 추가 (개발용 임시 엔드포인트)
    /// </summary>
    /// <returns>모든 Unit 목록</returns>
    [HttpPost("seed-units")]
    public IActionResult SeedUnits()
    {
        var random = new Random();
        var sampleUnits = new List<Unit>();

        foreach (var house in _houseTableRepo.Find(h => true)) // 모든 House 가져오기
        {
            int houseId = house.Id;

            // 각 크기별 유닛 생성
            int numberOfSizeL = random.Next(5, 9);
            sampleUnits.AddRange(GenerateUnits(houseId, UnitSize.L, numberOfSizeL));

            int numberOfSizeM = random.Next(10, 21);
            sampleUnits.AddRange(GenerateUnits(houseId, UnitSize.M, numberOfSizeM));

            int numberOfSizeS = random.Next(10, 16);
            sampleUnits.AddRange(GenerateUnits(houseId, UnitSize.S, numberOfSizeS));
        }

        _unitRepo.SetInitialData(sampleUnits);

        return Ok(new
                  {
                      Message = $"{sampleUnits.Count}개의 Unit 데이터가 초기화되었습니다.",
                      Units = sampleUnits
                  });
    }

    /// <summary>
    /// 예약 승인
    /// </summary>
    /// <param name="bookingId">승인할 Booking ID</param>
    /// <param name="request">승인에 필요한 지불 정보</param>
    /// <returns>승인 결과</returns>
    [HttpPost("approve-booking/{bookingId:int}")]
    public IActionResult ApproveBooking(int bookingId, [FromBody] BookingApprovalReqDto request)
    {
        Booking? booking = _bookingRepo.GetById(bookingId);
        if (booking == null)
        {
            return NotFound($"예약번호 {bookingId}에 해당하는 예약을 찾을 수 없습니다.");
        }

        Unit? unit = _unitRepo.GetById(booking.UnitId);
        if (unit == null)
        {
            return NotFound($"예약번호 {bookingId}에 해당하는 Unit을 찾을 수 없습니다.");
        }

        // Booking 승인 처리
        booking.Status = BookingStatus.Completed;
        booking.PaymentDate = request.PaymentDate;
        booking.PaymentMethod = request.PaymentMethod;
        booking.AdminNote = request.AdminNote;
        _bookingRepo.Update(booking);

        // Unit 상태 업데이트
        unit.Status = UnitStatus.InUse;
        _unitRepo.Update(unit);

        return Ok(new
                  {
                      Message = $"예약번호 {bookingId}이 성공적으로 승인되었습니다.",
                      Booking = booking,
                      Unit = unit
                  });
    }

    /// <summary>
    /// 예약 거부
    /// </summary>
    /// <param name="bookingId">거부할 Booking ID</param>
    /// <param name="request">거부사유(필수)가 포함된 데이터</param>
    /// <returns>거부 결과</returns>
    [HttpPost("reject-booking/{bookingId:int}")]
    public IActionResult RejectBooking(int bookingId, [FromBody] BookingRejectionReqDto request)
    {
        Booking? booking = _bookingRepo.GetById(bookingId);
        if (booking == null)
        {
            return NotFound($"예약번호 {bookingId}에 해당하는 예약을 찾을 수 없습니다.");
        }

        Unit? unit = _unitRepo.GetById(booking.UnitId);
        if (unit == null)
        {
            return NotFound($"예약번호 {bookingId}에 해당하는 Unit을 찾을 수 없습니다.");
        }

        booking.Status = BookingStatus.Rejected;
        booking.AdminNote = request.AdminNote;
        _bookingRepo.Update(booking);

        // Unit 상태 초기화
        unit.Status = UnitStatus.Available;
        unit.UserId = null;
        unit.StartDate = null;
        unit.EndDate = null;
        _unitRepo.Update(unit);

        return Ok(new
                  {
                      Message = $"예약 번호 {bookingId}이 거부되었습니다.",
                      Booking = booking,
                      Unit = unit
                  });
    }

    /// <summary>
    /// 특정 사이즈에대한 Unit 엔티티를 count만큼 생성한다. 
    /// </summary>
    /// <param name="houseId">유닛이 속한 House 레코드 Id</param>
    /// <param name="size">유닛크기(S,M,L)</param>
    /// <param name="count">이 사이즈의 유닛 총 갯수</param>
    /// <returns>Unit 모델 객체 목록</returns>
    private IEnumerable<Unit> GenerateUnits(int houseId, UnitSize size, int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new Unit
                         {
                             HouseId = houseId,
                             Size = size,
                             Status = UnitStatus.Available,
                             StartDate = null,
                             EndDate = null
                         };
        }
    }
}
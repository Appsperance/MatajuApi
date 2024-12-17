using MatajuApi.Models;

namespace MatajuApi.Data;

/// <summary>
/// 인메모리와 DB에서의 데이터 시딩을 위한 공통 인터페이스
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// 테이블 레포지토리에 레코드를 추가한다
    /// </summary>
    void SeedAllData();

    /// <summary>
    /// 임시 유저 시딩. 하나의 디폴트 User와 Admin을 추가한다.
    /// </summary>
    void SeedUsers();

    /// <summary>
    /// 창고 지점 데이터 시딩.이미 데이터가 있으면 추가하지 않는다.
    /// </summary>
    void SeedHouses();

    /// <summary>
    /// 창고지점별 창고유닛 데이터 시딩. 이미 데이터가 있으면 추가하지 않는다.
    /// </summary>
    void SeedUnits();
}
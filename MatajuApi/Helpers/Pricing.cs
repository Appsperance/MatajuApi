using MatajuApi.Models;

namespace MatajuApi.Helpers;

/// <summary>
/// 가격계산 헬퍼
/// </summary>
public static class Pricing
{
    /// <summary>
    /// 유닛가격을 30일가격으로 잡고 일단위로 쪼개서 기간만큼 최종 가격 계산 (10원 단위 반올림)
    /// </summary>
    /// <param name="pricePer30Days">30일치 가격(하우스별로 정해진 유닛 크기의 가격)</param>
    /// <param name="days">예약 기간 (일)</param>
    /// <returns>10원 단위 반올림되어 계산된 가격</returns>
    public static int CalculateBookingCharge(int pricePer30Days, int days)
    {
        decimal dailyRate = pricePer30Days / 30m;
        decimal totalPrice = dailyRate * days;

        // 10원 단위 반올림
        return (int)(Math.Round(totalPrice / 10, MidpointRounding.AwayFromZero) * 10);
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatajuApi.Models
{
    /// <summary>
    /// 사용자 테이블의 엔티티 모델 
    /// </summary>
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserId")]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///  password + salt가 해싱된 Base64 문자열
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        ///  해싱되기 전에 password 뒤에 추가된 24글자 문자열(base64)
        /// </summary>
        [Required]
        public string Salt { get; set; } = string.Empty;

        [Required]
        public string Nickname { get; set; } = string.Empty;

        [Required]
        public string Roles { get; set; } = "user";
    }
}
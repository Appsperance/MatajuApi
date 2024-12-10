using System.Security.Cryptography;
using System.Text;

namespace MatajuApi.Helpers
{
    /// <summary>
    /// 패스워드를 단방향 암호화(SHA256) 해싱하는 헬퍼
    /// </summary>
    public static class PwdHasher
    {
        /// <summary>
        /// 'password + 랜덤 salt'를 SHA256으로 해싱해서 DB에 저장가능한 Base64 문자열을 생성한다 
        /// </summary>
        /// <param name="targetPwd">해싱할 password</param>
        /// <param name="salt">유저의 비밀번호 마다 랜덤으로 생성되는 16바이트(24글자) Base64 문자열 (DB에 저장 필요) </param>
        /// <returns>해싱된 비밀번호 + salt 문자열 (Base64)</returns>
        public static string GenerateHash(string targetPwd, out string salt)
        {
            //16바이트 (24글자) 랜덤 솔트 생성 
            salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

            using (var sha256 = SHA256.Create())
            {
                byte[] saltedPwd = Encoding.UTF8.GetBytes(targetPwd + salt);
                byte[] hashedPwd = sha256.ComputeHash(saltedPwd);
                return Convert.ToBase64String(hashedPwd);
            }
        }

        /// <summary>
        /// password가 맞는지 확인한다.
        /// </summary>
        /// <param name="targetPwd">비교할 password 문자열(Raw Text)</param>
        /// <param name="storedSalt">유저의 password를 해싱할 당시 사용된 salt</param>
        /// <param name="storedPwdHash">DB에 저장된 유저의 password + salt의 해싱 문자열(base64)</param>
        /// <returns>DB에 저장해둔 해싱된 비밀번호 문자열과 비교결과</returns>
        public static bool VerifyHash(string targetPwd, string storedSalt, string storedPwdHash)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] saltedTargetPwd = Encoding.UTF8.GetBytes(targetPwd + storedSalt);
                byte[] hashedTargetPwd = sha256.ComputeHash(saltedTargetPwd);
                return Convert.ToBase64String(hashedTargetPwd) == storedPwdHash;
            }
        }
    }
}
using Stakraft.HostSystem.Service.ServiceDto.SFTP;

namespace Stakraft.HostSystem.Service.Service
{
    public interface IAuxFtpService
    {
        bool TransferFile(string fileName, byte[] file, SftpDto connectionParams);
        string getContentFileOut(SftpDto connectionParams, string fileName);
        string getContentFileOutBbva(SftpDto connectionParams, string fileName);
    }
}

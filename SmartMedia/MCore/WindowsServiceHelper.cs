using Microsoft.Win32;
using System.Diagnostics;
using System.ServiceProcess;

namespace SmartMedia.MCore;

/// <summary>  
/// Windows 服务管理助手类（使用 sc.exe 命令行工具）
/// <para>使用说明：</para>
/// <para>1. 使用 sc.exe 命令行工具进行服务的安装、卸载、启动、停止等操作</para>
/// <para>2. 需要管理员权限才能执行这些操作</para>
/// </summary>   
public class WindowsServiceHelper
{
    #region 安装服务

    /// <summary>    
    /// 安装服务    
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="displayName">服务显示名称</param>
    /// <param name="executablePath">可执行文件完整路径</param>
    /// <param name="description">服务描述（可选）</param>
    /// <returns>安装成功返回 true，否则返回 false</returns>
    public static bool InstallService(string serviceName, string displayName, string executablePath, string? description = null)
    {
        if (IsServiceExisted(serviceName))
        {
            return true;
        }

        try
        {
            // 使用 sc create 命令创建服务
            // sc create [服务名] binPath= [可执行文件路径] DisplayName= [显示名称] start= auto
            var arguments = $"create \"{serviceName}\" binPath= \"{executablePath}\" DisplayName= \"{displayName}\" start= auto";
            var result = ExecuteScCommand(arguments);

            if (!result)
            {
                return false;
            }

            // 如果提供了描述，设置服务描述
            if (!string.IsNullOrWhiteSpace(description))
            {
                SetServiceDescription(serviceName, description);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region 卸载服务

    /// <summary>    
    /// 卸载服务    
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <returns>卸载成功返回 true，否则返回 false</returns>
    public static bool UninstallService(string serviceName)
    {
        if (!IsServiceExisted(serviceName))
        {
            return true;
        }

        try
        {
            // 先停止服务
            StopService(serviceName);

            // 使用 sc delete 命令删除服务
            var arguments = $"delete \"{serviceName}\"";
            return ExecuteScCommand(arguments);
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region 检查服务存在性

    /// <summary>    
    /// 检查服务是否存在    
    /// </summary>    
    /// <param name="serviceName">服务名</param>    
    /// <returns>存在返回 true，否则返回 false</returns>    
    public static bool IsServiceExisted(string serviceName)
    {
        try
        {
            var services = ServiceController.GetServices();
            return services.Any(s =>
                s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region 判断服务是否启动

    /// <summary>    
    /// 判断某个 Windows 服务是否正在运行    
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <returns>已启动返回 true，否则返回 false</returns>
    public static bool IsServiceRunning(string serviceName)
    {
        try
        {
            using var service = new ServiceController(serviceName);
            return service.Status == ServiceControllerStatus.Running;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region 获取服务状态

    /// <summary>
    /// 获取服务当前状态
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <returns>服务状态，如果服务不存在返回 null</returns>
    public static ServiceControllerStatus? GetServiceStatus(string serviceName)
    {
        try
        {
            using var service = new ServiceController(serviceName);
            return service.Status;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region 修改服务启动类型

    /// <summary>      
    /// 修改服务的启动类型      
    /// </summary>      
    /// <param name="serviceName">服务名称</param>
    /// <param name="startType">启动类型：auto=自动，demand=手动，disabled=禁用</param>      
    /// <returns>修改成功返回 true，否则返回 false</returns>      
    public static bool ChangeServiceStartType(string serviceName, string startType)
    {
        try
        {
            // 使用 sc config 命令修改启动类型
            var arguments = $"config \"{serviceName}\" start= {startType}";
            return ExecuteScCommand(arguments);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>      
    /// 修改服务的启动类型（使用注册表方式）     
    /// </summary>      
    /// <param name="serviceName">服务名称</param>      
    /// <param name="startType">启动类型：2=自动，3=手动，4=禁用</param>      
    /// <returns>修改成功返回 true，否则返回 false</returns>      
    public static bool ChangeServiceStartType(string serviceName, int startType)
    {
        try
        {
            using var servicesKey = Registry.LocalMachine
                .OpenSubKey(@"SYSTEM\CurrentControlSet\Services")?
                .OpenSubKey(serviceName, true);

            if (servicesKey is null)
            {
                return false;
            }

            servicesKey.SetValue("Start", startType);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region 启动服务

    /// <summary>
    /// 启动服务
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="timeoutSeconds">超时时间（秒），默认 60 秒</param>
    /// <returns>启动成功返回 true，否则返回 false</returns>
    public static bool StartService(string serviceName, int timeoutSeconds = 60)
    {
        if (!IsServiceExisted(serviceName))
        {
            return false;
        }

        try
        {
            using var service = new ServiceController(serviceName);

            if (service.Status is ServiceControllerStatus.Running
                or ServiceControllerStatus.StartPending)
            {
                return true;
            }

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running,
                TimeSpan.FromSeconds(timeoutSeconds));

            return service.Status == ServiceControllerStatus.Running;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region 停止服务

    /// <summary>
    /// 停止服务
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="timeoutSeconds">超时时间（秒），默认 60 秒</param>
    /// <returns>停止成功返回 true，否则返回 false</returns>
    public static bool StopService(string serviceName, int timeoutSeconds = 60)
    {
        if (!IsServiceExisted(serviceName))
        {
            return false;
        }

        try
        {
            using var service = new ServiceController(serviceName);

            if (service.Status is ServiceControllerStatus.Stopped
                or ServiceControllerStatus.StopPending)
            {
                return true;
            }

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped,
                TimeSpan.FromSeconds(timeoutSeconds));

            return service.Status == ServiceControllerStatus.Stopped;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region 重启服务

    /// <summary>
    /// 重启服务
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="timeoutSeconds">超时时间（秒），默认 60 秒</param>
    /// <returns>重启成功返回 true，否则返回 false</returns>
    public static bool RestartService(string serviceName, int timeoutSeconds = 60)
    {
        return StopService(serviceName, timeoutSeconds) &&
               StartService(serviceName, timeoutSeconds);
    }

    #endregion

    #region 设置服务描述

    /// <summary>
    /// 设置服务描述
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="description">服务描述</param>
    /// <returns>设置成功返回 true，否则返回 false</returns>
    public static bool SetServiceDescription(string serviceName, string description)
    {
        try
        {
            // 使用 sc description 命令设置描述
            var arguments = $"description \"{serviceName}\" \"{description}\"";
            return ExecuteScCommand(arguments);
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region 执行 sc 命令

    /// <summary>
    /// 执行 sc.exe 命令
    /// </summary>
    /// <param name="arguments">命令参数</param>
    /// <returns>执行成功返回 true，否则返回 false</returns>
    private static bool ExecuteScCommand(string arguments)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            process.WaitForExit();

            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}
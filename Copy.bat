copy Daylily.Plugin.Kernel\bin\Debug\netcoreapp2.2\Daylily.Plugin.Kernel.dll Daylily.AspNetCore\bin\Debug\netcoreapp2.2\Plugin\Daylily.Plugin.Kernel.dll
copy Daylily.Plugin.ShaDiao\bin\Debug\netcoreapp2.2\Daylily.Plugin.ShaDiao.dll Daylily.AspNetCore\bin\Debug\netcoreapp2.2\Plugin\Daylily.Plugin.ShaDiao.dll
copy Daylily.Plugin.Core\bin\Debug\netcoreapp2.2\Daylily.Plugin.Core.dll Daylily.AspNetCore\bin\Debug\netcoreapp2.2\Plugin\Daylily.Plugin.Core.dll
copy Daylily.Plugin.Osu\bin\Debug\netcoreapp2.2\Daylily.Plugin.Osu.dll Daylily.AspNetCore\bin\Debug\netcoreapp2.2\Plugin\Daylily.Plugin.Osu.dll 
copy Daylily.Plugin.Kernel\bin\Release\netcoreapp2.2\Daylily.Plugin.Kernel.dll Daylily.AspNetCore\bin\Release\PublishOutput\Plugin\Daylily.Plugin.Kernel.dll
copy Daylily.Plugin.ShaDiao\bin\Release\netcoreapp2.2\Daylily.Plugin.ShaDiao.dll Daylily.AspNetCore\bin\Release\PublishOutput\Plugin\Daylily.Plugin.ShaDiao.dll
copy Daylily.Plugin.Core\bin\Release\netcoreapp2.2\Daylily.Plugin.Core.dll Daylily.AspNetCore\bin\Release\PublishOutput\Plugin\Daylily.Plugin.Core.dll
copy Daylily.Plugin.Osu\bin\Release\netcoreapp2.2\Daylily.Plugin.Osu.dll Daylily.AspNetCore\bin\Release\PublishOutput\Plugin\Daylily.Plugin.Osu.dll
@echo off
ping -n 1 123.45.67.89 -w 1500 > nul
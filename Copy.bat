copy Daylily.Plugin.ShaDiao\bin\Debug\netcoreapp2.1\Daylily.Plugin.ShaDiao.dll Daylily.Web\bin\Debug\netcoreapp2.1\Plugin\Daylily.Plugin.ShaDiao.dll
copy Daylily.Plugin.Core\bin\Debug\netcoreapp2.1\Daylily.Plugin.Core.dll Daylily.Web\bin\Debug\netcoreapp2.1\Plugin\Daylily.Plugin.Core.dll
copy Daylily.Plugin.Osu\bin\Debug\netcoreapp2.1\Daylily.Plugin.Osu.dll Daylily.Web\bin\Debug\netcoreapp2.1\Plugin\Daylily.Plugin.Osu.dll
copy Daylily.Plugin.ShaDiao\bin\Release\netcoreapp2.1\Daylily.Plugin.ShaDiao.dll Daylily.Web\bin\Release\PublishOutput\Plugin\Daylily.Plugin.ShaDiao.dll
copy Daylily.Plugin.Core\bin\Release\netcoreapp2.1\Daylily.Plugin.Core.dll Daylily.Web\bin\Release\PublishOutput\Plugin\Daylily.Plugin.Core.dll
copy Daylily.Plugin.Osu\bin\Release\netcoreapp2.1\Daylily.Plugin.Osu.dll Daylily.Web\bin\Release\PublishOutput\Plugin\Daylily.Plugin.Osu.dll
@echo off
ping -n 1 123.45.67.89 -w 1500 > nul
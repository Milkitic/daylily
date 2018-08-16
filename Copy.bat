copy Daylily.Plugin.ShaDiao\bin\Debug\netcoreapp2.1\Daylily.Plugin.ShaDiao.dll Daylily.Web\bin\Debug\netcoreapp2.1\plugins\Daylily.Plugin.ShaDiao.dll
copy Daylily.Plugin.Core\bin\Debug\netcoreapp2.1\Daylily.Plugin.Core.dll Daylily.Web\bin\Debug\netcoreapp2.1\plugins\Daylily.Plugin.Core.dll
copy Daylily.Plugin.Osu\bin\Debug\netcoreapp2.1\Daylily.Plugin.Osu.dll Daylily.Web\bin\Debug\netcoreapp2.1\plugins\Daylily.Plugin.Osu.dll
copy Daylily.Plugin.ShaDiao\bin\Release\netcoreapp2.1\Daylily.Plugin.ShaDiao.dll Daylily.Web\bin\Release\PublishOutput\plugins\Daylily.Plugin.ShaDiao.dll
copy Daylily.Plugin.Core\bin\Release\netcoreapp2.1\Daylily.Plugin.Core.dll Daylily.Web\bin\Release\PublishOutput\plugins\Daylily.Plugin.Core.dll
copy Daylily.Plugin.Osu\bin\Release\netcoreapp2.1\Daylily.Plugin.Osu.dll Daylily.Web\bin\Release\PublishOutput\plugins\Daylily.Plugin.Osu.dll
@echo off
ping -n 3 127.0.0.1>nul
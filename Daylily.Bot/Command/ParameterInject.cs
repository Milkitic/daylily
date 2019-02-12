using Daylily.Bot.Backend;
using Daylily.Bot.Messaging;
using Daylily.Common.Logging;
using System;
using System.Reflection;

namespace Daylily.Bot.Command
{
    public static class ParameterInject
    {
        public static bool TryInjectParameters(this IInjectableBackend backend, IArgument command, out BindingFailedItem bindingFailedItem)
        {
            PropertyInfo[] props;
            if (backend.Parameters == null)
            {
                props = backend.GetType().GetProperties();
                return InnerInject(backend, command, out bindingFailedItem, props);
            }
            else
            {
                props = backend.Parameters.GetType().GetProperties();
                return InnerInject(backend.Parameters, command, out bindingFailedItem, props);
            }
        }

        private static bool InnerInject(object hostInstance, IArgument command, out BindingFailedItem bindingFailedItem,
            PropertyInfo[] props)
        {
            bindingFailedItem = null;
            int freeIndex = 0;
            string[] freeArray = command.FreeArgs.ToArray();
            int freeCount = freeArray.Length;
            int swCount = command.Switches.Count;
            int argCount = command.Args.Count;

            foreach (var prop in props)
            {
                var infos = prop.GetCustomAttributes(false);
                if (infos.Length == 0) continue;
                foreach (var info in infos)
                {
                    switch (info)
                    {
                        case ArgAttribute argAttr:
                            if (command.Switches.Contains(argAttr.Name))
                            {
                                if (argAttr.IsSwitch)
                                {
                                    prop.SetValue(hostInstance, true);
                                    swCount--;
                                }
                            }
                            else if (command.Args.ContainsKey(argAttr.Name))
                            {
                                if (!argAttr.IsSwitch)
                                {
                                    if (TryParse(prop, command.Args[argAttr.Name], out var parsed))
                                    {
                                        //dynamic obj = TryParse(prop, cm.Args[argAttrib.Name]);
                                        prop.SetValue(hostInstance, parsed);
                                        argCount--;
                                    }
                                    else
                                    {
                                        bindingFailedItem = new BindingFailedItem
                                        {
                                            Parameter = "sdfg"
                                        };

                                        //SendMessage(
                                        //    routeMsg.ToSource($"参数有误...发送 \"/help {cm.Command}\" 了解如何使用。", cm));
                                        return false;
                                    }
                                }
                            }
                            else if (argAttr.Default != null)
                            {
                                prop.SetValue(hostInstance, argAttr.Default); //不再转换，提升效率
                            }

                            break;
                        case FreeArgAttribute freeArgAttr:
                        {
                            if (freeIndex > freeCount - 1)
                            {
                                if (freeArgAttr.Default != null)
                                    prop.SetValue(hostInstance, freeArgAttr.Default); //不再转换，提升效率
                                break;
                            }

                            if (TryParse(prop, freeArray[freeIndex], out var parsed))
                            {
                                prop.SetValue(hostInstance, parsed);
                                freeIndex++;
                                break;
                            }
                            else
                            {
                                //SendMessage(routeMsg.ToSource($"参数有误...发送 \"/help {cm.Command}\" 了解如何使用。",
                                //    cm));
                                bindingFailedItem = new BindingFailedItem
                                {
                                    Parameter = "sdfg"
                                };

                                return false;
                            }
                        }
                    }
                }
            }

            if (swCount <= 0 && argCount <= 0)
                return true;
            bindingFailedItem = new BindingFailedItem
            {
                Parameter = "sdfg"
            };
            return false;
        }

        private static bool TryParse(PropertyInfo prop, string argStr, out dynamic parsed)
        {
            try
            {
                if (prop.PropertyType == typeof(int))
                {
                    parsed = Convert.ToInt32(argStr);
                }
                else if (prop.PropertyType == typeof(long))
                {
                    parsed = Convert.ToInt64(argStr);
                }
                else if (prop.PropertyType == typeof(short))
                {
                    parsed = Convert.ToInt16(argStr);
                }
                else if (prop.PropertyType == typeof(float))
                {
                    parsed = Convert.ToSingle(argStr);
                }
                else if (prop.PropertyType == typeof(double))
                {
                    parsed = Convert.ToDouble(argStr);
                }
                else if (prop.PropertyType == typeof(string))
                {
                    parsed = argStr; // Convert.ToString(cmd);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    string tmpCmd = argStr == "" ? "true" : argStr;
                    if (tmpCmd == "0")
                        tmpCmd = "false";
                    else if (tmpCmd == "1")
                        tmpCmd = "true";
                    parsed = Convert.ToBoolean(tmpCmd);
                }
                else
                {
                    throw new NotSupportedException("sb");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                parsed = null;
                return false;
            }
        }
    }
}

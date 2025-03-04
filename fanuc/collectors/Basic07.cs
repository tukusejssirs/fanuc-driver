﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using l99.driver.@base;
using Newtonsoft.Json.Linq;

namespace l99.driver.fanuc.collectors
{
    public class Basic07 : FanucCollector
    {
        private Stopwatch _sweepWatch = new Stopwatch();
        private int _sweepRemaining = 1000;
        public Basic07(Machine machine, int sweepMs = 1000, params dynamic[] additional_params) : base(machine, sweepMs, additional_params)
        {
            _sweepRemaining = sweepMs;
        }
        
        public override async Task SweepAsync(int delayMs = -1)
        {
            _sweepRemaining = _sweepMs - (int)_sweepWatch.ElapsedMilliseconds;
            if (_sweepRemaining < 0)
            {
                _sweepRemaining = _sweepMs;
            }
            _logger.Trace($"[{_machine.Id}] Sweep delay: {_sweepRemaining}ms");

            await base.SweepAsync(_sweepRemaining);
        }
        
        public override async Task<dynamic?> InitializeAsync()
        {
            try
            {
                while (!_machine.VeneersApplied)
                {
                    dynamic connect = await _platform.ConnectAsync();
                    
                    if (connect.success)
                    {
                        _machine.ApplyVeneer(typeof(fanuc.veneers.FocasPerf), "focas_perf", true);
                        _machine.ApplyVeneer(typeof(fanuc.veneers.Connect), "connect");
                        _machine.ApplyVeneer(typeof(fanuc.veneers.CNCId), "cnc_id");
                        _machine.ApplyVeneer(typeof(fanuc.veneers.RdTimer), "power_on_time");
                        _machine.ApplyVeneer(typeof(fanuc.veneers.RdParamLData), "power_on_time_6750");
                        _machine.ApplyVeneer(typeof(fanuc.veneers.GetPath), "get_path");
                        
                        dynamic paths = await _platform.GetPathAsync();

                        IEnumerable<int> path_slices = Enumerable
                            .Range(paths.response.cnc_getpath.path_no, paths.response.cnc_getpath.maxpath_no);

                        _machine.SliceVeneer(path_slices.ToArray());

                        _machine.ApplyVeneerAcrossSlices(typeof(fanuc.veneers.SysInfo), "sys_info");
                        _machine.ApplyVeneerAcrossSlices(typeof(fanuc.veneers.StatInfo), "stat_info");
                        _machine.ApplyVeneerAcrossSlices(typeof(fanuc.veneers.GCodeBlocks), "gcode_blocks");
                        _machine.ApplyVeneerAcrossSlices(typeof(fanuc.veneers.Figures), "figures");
                        _machine.ApplyVeneerAcrossSlices(typeof(fanuc.veneers.RdAxisname), "axis_name");
                        _machine.ApplyVeneerAcrossSlices(typeof(fanuc.veneers.RdSpindlename), "spindle_name");
                        
                        for (short current_path = paths.response.cnc_getpath.path_no;
                            current_path <= paths.response.cnc_getpath.maxpath_no;
                            current_path++)
                        {
                            dynamic path = await _platform.SetPathAsync(current_path);
                            
                            dynamic axes = await _platform.RdAxisNameAsync();
                            dynamic spindles = await _platform.RdSpdlNameAsync();
                            dynamic axis_spindle_slices = new List<dynamic> { };

                            var fields_axes = axes.response.cnc_rdaxisname.axisname.GetType().GetFields();
                            for (int x = 0; x <= axes.response.cnc_rdaxisname.data_num - 1; x++)
                            {
                                var axis = fields_axes[x].GetValue(axes.response.cnc_rdaxisname.axisname);
                                axis_spindle_slices.Add(AxisName(axis));
                            }
                            
                            var fields_spindles = spindles.response.cnc_rdspdlname.spdlname.GetType().GetFields();
                            for (int x = 0; x <= spindles.response.cnc_rdspdlname.data_num - 1; x++)
                            {
                                var spindle = fields_spindles[x].GetValue(spindles.response.cnc_rdspdlname.spdlname);
                                axis_spindle_slices.Add(SpindleName(spindle));
                            };

                            _machine.SliceVeneer(current_path, axis_spindle_slices.ToArray());

                            // the RdDynamic2_1 veneer is an extension of RdDynamic2 veneer
                            //  the difference is that RdDynamic2_1 will use output from the Figures veneer
                            //  to determine the correct decimal position for axis position data
                            _machine.ApplyVeneerAcrossSlices(current_path, typeof(fanuc.veneers.RdDynamic2_1), "axis_data");
                            _machine.ApplyVeneerAcrossSlices(current_path, typeof(fanuc.veneers.RdActs2), "spindle_data");
                        }
                        
                        dynamic disconnect = await _platform.DisconnectAsync();
                        
                        _machine.VeneersApplied = true;
                    }
                    else
                    {
                        await Task.Delay(_sweepMs);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{_machine.Id}] Collector initialization failed.");
            }

            return null;
        }

        public override async Task<dynamic?> CollectAsync()
        {
            try
            {
                _sweepWatch.Restart();

                dynamic focas_invocations = new List<dynamic>();
                
                Action<dynamic> catch_focas_perf = (ret) =>
                {
                    focas_invocations.Add(new
                    {
                        ret.method,
                        ret.invocationMs,
                        ret.rc
                    });
                };
                
                dynamic connect = await _platform.ConnectAsync();
                await _machine.PeelVeneerAsync("connect", connect);
                catch_focas_perf(connect);

                if (connect.success)
                {
                    dynamic cncid = await _platform.CNCIdAsync();
                    await _machine.PeelVeneerAsync("cnc_id", cncid);
                    catch_focas_perf(cncid);
                    
                    dynamic poweron = await _platform.RdTimerAsync(0);
                    await _machine.PeelVeneerAsync("power_on_time", poweron);
                    catch_focas_perf(poweron);
                    
                    dynamic poweron_6750 = await _platform.RdParamDoubleWordNoAxisAsync(6750);
                    await _machine.PeelVeneerAsync("power_on_time_6750", poweron_6750);
                    catch_focas_perf(poweron_6750);
                    
                    dynamic paths = await _platform.GetPathAsync();
                    await _machine.PeelVeneerAsync("get_path", paths);
                    catch_focas_perf(paths);

                    for (short current_path = paths.response.cnc_getpath.path_no;
                        current_path <= paths.response.cnc_getpath.maxpath_no;
                        current_path++)
                    {
                        dynamic path = await _platform.SetPathAsync(current_path);
                        dynamic path_marker = PathMarker(path);
                        
                        _machine.MarkVeneer(current_path, path_marker);
                        catch_focas_perf(path);
                        
                        dynamic info = await _platform.SysInfoAsync();
                        await _machine.PeelAcrossVeneerAsync(current_path,"sys_info", info);
                        catch_focas_perf(info);
                        
                        dynamic stat = await _platform.StatInfoAsync();
                        await _machine.PeelAcrossVeneerAsync(current_path, "stat_info", stat);
                        catch_focas_perf(stat);
                        
                        dynamic blkcount = await _platform.RdBlkCountAsync();
                        catch_focas_perf(blkcount);
                        
                        dynamic actpt = await _platform.RdActPtAsync();
                        catch_focas_perf(actpt);
                        
                        dynamic execprog = await _platform.RdExecProgAsync(128);
                        catch_focas_perf(execprog);
                        
                        /*await _machine.PeelAcrossVeneerAsync(current_path, "gcode_blocks", new
                        {
                            success = blkcount.success && actpt.success && execprog.success,
                            blkcount.response.cnc_rdblkcount.prog_bc,
                            actpt.response.cnc_rdactpt.blk_no,
                            execprog.response.cnc_rdexecprog.data
                        });*/
                        
                        await _machine.PeelAcrossVeneerAsync(current_path, "gcode_blocks", 
                            blkcount,
                            actpt, execprog);
                        
                        dynamic figures = await _platform.GetFigureAsync(0, 32);
                        await _machine.PeelAcrossVeneerAsync(current_path,"figures", figures);
                        catch_focas_perf(figures);
                        
                        dynamic axes = await _platform.RdAxisNameAsync();
                        await _machine.PeelAcrossVeneerAsync(current_path, "axis_name", axes);
                        catch_focas_perf(axes);

                        dynamic spindles = await _platform.RdSpdlNameAsync();
                        await _machine.PeelAcrossVeneerAsync(current_path, "spindle_name", spindles);
                        catch_focas_perf(spindles);
                        
                        var fields_axes = axes.response.cnc_rdaxisname.axisname.GetType().GetFields();

                        for (short current_axis = 1;
                            current_axis <= axes.response.cnc_rdaxisname.data_num;
                            current_axis++)
                        {
                            dynamic axis = fields_axes[current_axis-1].GetValue(axes.response.cnc_rdaxisname.axisname);
                            dynamic axis_name = AxisName(axis);
                            dynamic axis_marker = AxisMarker(axis);
                            dynamic axis_split = new[] {current_path, axis_name};
                            
                            _machine.MarkVeneer(axis_split, new[] { path_marker, axis_marker });
                            
                            // the figures observation determines where the decimal point goes in axis positional data
                            //  we pass it as input2 to reveal the 'axis_data' observation, along with the axis index
                            //  and do the math inside RdDynamic2_1 veneer
                            dynamic axis_data = await _platform.RdDynamic2Async(current_axis, 44, 2);
                            await _machine.PeelAcrossVeneerAsync(axis_split, 
                                "axis_data", 
                                axis_data,
                                figures, current_axis - 1);
                            catch_focas_perf(axis_data);
                        }

                        var fields_spindles = spindles.response.cnc_rdspdlname.spdlname.GetType().GetFields();
                        
                        for (short current_spindle = 1;
                            current_spindle <= spindles.response.cnc_rdspdlname.data_num;
                            current_spindle++)
                        {
                            var spindle = fields_spindles[current_spindle - 1].GetValue(spindles.response.cnc_rdspdlname.spdlname);
                            dynamic spindle_name = SpindleName(spindle);
                            dynamic spindle_marker = SpindleMarker(spindle);
                            dynamic spindle_split = new[] {current_path, spindle_name};
                            
                            _machine.MarkVeneer(spindle_split, new[] { path_marker, spindle_marker });
                            
                            dynamic spindle_data = await _platform.Acts2Async(current_spindle);
                            await _machine.PeelAcrossVeneerAsync(spindle_split, "spindle_data", spindle_data);
                            catch_focas_perf(spindle_data);
                        };
                    }

                    dynamic disconnect = await _platform.DisconnectAsync();
                    catch_focas_perf(disconnect);

                    await _machine.PeelVeneerAsync("focas_perf", new
                    {
                        sweepMs = _sweepWatch.ElapsedMilliseconds,
                        focas_invocations
                    });
                    
                    LastSuccess = connect.success;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{_machine.Id}] Collector sweep failed.");
            }

            return null;
        }
    }
}
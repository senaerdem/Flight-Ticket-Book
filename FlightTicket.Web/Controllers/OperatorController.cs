using FlightTicket.Business.Abstract;
using FlightTicket.Core;
using FlightTicket.Entity;
using FlightTicket.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightTicket.Web.Controllers
{
    [Authorize(Roles = "Operator")]
    public class OperatorController : Controller
    {
        private readonly ITripService _TripService;
        private readonly ILineService _LineService;
        private readonly IFlightService _FlightService;
        private readonly IDriverService _DriverService;
        private readonly ITripDetailService _TripDetailService;
        private readonly IMidlineService _MidlineService;
        public OperatorController(ITripService TripService, ILineService lineService, IFlightService flightService, IDriverService driverService, IMidlineService midlineService, ITripDetailService tripDetailModel)
        {
            _TripService = TripService;
            _LineService = lineService;
            _FlightService = flightService;
            _DriverService = driverService;
            _TripDetailService = tripDetailModel;
            _MidlineService = midlineService;
        }

        #region LineTripActions

        //public async Task<IActionResult> TripList()
        //{
        //    var trips = await _TripService.GetAllTripsWDetails();
        //    return View(trips);
        //}

        public async Task<IActionResult> UpdateLine(int id)
        {
            Line line = await _LineService.GetLineWithDetailsAsync(id);
            List<MidLine> midlines = line.MidLines;
            List<Trip> trips = new();
            foreach (MidLine midline in midlines)
            {
                trips.Add(midline.Trips.First());
            };
            var tripDetail = trips.First().TripDetail;
            UpdateLineModel updateLineModel = new()
            {
                LineId = line.Id,
                Flightes = await _FlightService.GetAllAsync(),
                Drivers = await _DriverService.GetAllAsync(),
                Date = Jobs.UpdateDateFormatToInput(trips.First().ScheduleDate),
                DriverId = tripDetail.DriverId,
                FlightId = tripDetail.FlightId,
                Stops = new List<string>(),
                Time = new List<string>(),
                Fares = new List<string>(),
                StopTimeFares = new List<StopTimeFareModel>()
            };
            for (int i = 0; i < midlines.Count; i++)
            {
                StopTimeFareModel stopTimeFare = new()
                {
                    Stop = midlines[i].StartingPoint,
                    Time = midlines[i].Trips.First().DepartureTime,
                    Fare = (midlines[i].Trips.First().FareAmount).ToString()
                };
                updateLineModel.StopTimeFares.Add(stopTimeFare);
                if (i == midlines.Count - 1)
                {
                    StopTimeFareModel stopTimeFareLast = new()
                    {
                        Stop = midlines[i].Destination,
                        Time = midlines[i].Trips.First().ArrivalTime,
                    };
                    updateLineModel.StopTimeFares.Add(stopTimeFareLast);
                }
            };
            return View(updateLineModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLine(UpdateLineModel updateLineModel)
        {

            if (updateLineModel.Stops.Contains(null) || updateLineModel.Time.Contains(null) || updateLineModel.Fares.Contains(null) || updateLineModel.Stops.Contains("") || updateLineModel.Time.Contains("") || updateLineModel.Fares.Contains("") || !ModelState.IsValid)
            {
                if (updateLineModel.Stops.Contains(null) || updateLineModel.Stops.Contains(""))
                {
                    ModelState.AddModelError("", "All stops must be filled");
                }
                if (updateLineModel.Time.Contains(null) || updateLineModel.Time.Contains(""))
                {
                    ModelState.AddModelError("", "All Time inputs must be filled");
                }
                if (updateLineModel.Fares.Contains(null) || updateLineModel.Fares.Contains(""))
                {
                    ModelState.AddModelError("", "All Fares inputs must be filled");
                }
                var stopTimeFares = new List<StopTimeFareModel>();
                for (int i = 0; i < updateLineModel.Stops.Count; i++)
                {
                    var stopTimeFare = new StopTimeFareModel()
                    {
                        Stop = updateLineModel.Stops[i],
                        Time = updateLineModel.Time[i],
                        Fare = i == updateLineModel.Stops.Count - 1 ? null : updateLineModel.Fares[i]
                    };
                    stopTimeFares.Add(stopTimeFare);
                }
                updateLineModel.StopTimeFares = stopTimeFares;
                updateLineModel.Drivers = await _DriverService.GetAllAsync();
                updateLineModel.Flightes = await _FlightService.GetAllAsync();
                return View(updateLineModel);
            }

            var line = await _LineService.GetLineWithDetailsAsync(updateLineModel.LineId);
            var midlines = line.MidLines;
            List<Trip> trips = new();
            foreach (var midline in midlines)
            {
                trips.Add(midline.Trips.First());
            }
            var tripDetail = trips.First().TripDetail;

            tripDetail.DriverId = (int)updateLineModel.DriverId;
            tripDetail.FlightId = (int)updateLineModel.FlightId;

            if (updateLineModel.Stops.Count == midlines.Count + 1)
            {
                line.StartingPoint = updateLineModel.Stops.First();
                line.Destination = updateLineModel.Stops.Last();
                await _LineService.UpdateAsync(line);
                for (int i = 0; i < midlines.Count; i++)
                {
                    midlines[i].StartingPoint = updateLineModel.Stops[i];
                    midlines[i].Destination = updateLineModel.Stops[(i + 1)];
                    await _MidlineService.UpdateAsync(midlines[i]);

                    trips[i].DepartureTime = updateLineModel.Time[i];
                    trips[i].ArrivalTime = updateLineModel.Time[i + 1];
                    trips[i].ScheduleDate = Jobs.UpdateDateFormat(updateLineModel.Date);
                    trips[i].FareAmount = decimal.Parse(updateLineModel.Fares[i]);
                    await _TripService.UpdateAsync(trips[i]);

                }
            }
            else
            {
                do
                {
                    await _MidlineService.DeleteAsync(midlines.First());
                } while (midlines.Count > 0);

                for (int i = 0; i < (updateLineModel.Stops.Count - 1); i++)
                {
                    MidLine midline = new()
                    {
                        MidLineOrder = i + 1,
                        StartingPoint = updateLineModel.Stops[i],
                        Destination = updateLineModel.Stops[i + 1],
                        LineId = line.Id
                    };
                    await _MidlineService.CreateAsync(midline);
                }

                line = await _LineService.GetLineWithDetailsAsync(line.Id);

                for (int x = 0; x < line.MidLines.Count; x++)
                {
                    Trip trip = new()
                    {
                        MidLineId = line.MidLines[x].Id,
                        ScheduleDate = Jobs.UpdateDateFormat(updateLineModel.Date),
                        DepartureTime = updateLineModel.Time[x],
                        ArrivalTime = updateLineModel.Time[x + 1],
                        FareAmount = decimal.Parse(updateLineModel.Fares[x]),
                        TripDetailId = tripDetail.Id
                    };
                    await _TripService.CreateAsync(trip);
                }

            }
                return RedirectToAction("LineList");
        }
        public async Task<IActionResult> LineList()
        {
            var lines = await _LineService.GetLinesWithTripsAsync();
            return View(lines);
        }

        public async Task<IActionResult> LineDetails(int id)
        {
            var line = await _LineService.GetLineWithDetailsAsync(id);
            return View(line);
        }

        public async Task<IActionResult> CreateLine()
        {
            var createLineModel = new CreateLineModel
            {
                Flightes = await _FlightService.GetAllAsync(),
                Drivers = await _DriverService.GetAllAsync()
            };
            return View(createLineModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateLine(CreateLineModel createLine)
        {
            if (createLine.Stops.Contains(null) || createLine.Time.Contains(null) || createLine.Fares.Contains(null) || !ModelState.IsValid)
            {
                if (createLine.Stops.Contains(null))
                {
                    ModelState.AddModelError("", "All stops must be filled");
                }
                if (createLine.Time.Contains(null))
                {
                    ModelState.AddModelError("", "All Time inputs must be filled");
                }
                if (createLine.Fares.Contains(null))
                {
                    ModelState.AddModelError("", "All Fares inputs must be filled");
                }
                var stopTimeFares = new List<StopTimeFareModel>();
                for (int i = 0; i < createLine.Stops.Count; i++)
                {
                    var stopTimeFare = new StopTimeFareModel()
                    {
                        Stop = createLine.Stops[i],
                        Time = createLine.Time[i],
                        Fare = i == createLine.Stops.Count - 1 ? null : createLine.Fares[i]
                    };
                    stopTimeFares.Add(stopTimeFare);
                }
                createLine.StopTimeFares = stopTimeFares;
                createLine.Drivers = await _DriverService.GetAllAsync();
                createLine.Flightes = await _FlightService.GetAllAsync();
                return View(createLine);
            }

            var line = new Line()
            {

                Destination = createLine.Stops.Last(),
                StartingPoint = createLine.Stops.First()
            };

            await _LineService.CreateAsync(line);

            for (int i = 0; i < (createLine.Stops.Count - 1); i++)
            {
                MidLine midline = new()
                {
                    MidLineOrder = i + 1,
                    StartingPoint = createLine.Stops[i],
                    Destination = createLine.Stops[i + 1],
                    LineId = line.Id
                };
                await _MidlineService.CreateAsync(midline);
            }

            TripDetail tripDetail = new()
            {
                FlightId = (int)createLine.FlightId,
                DriverId = (int)createLine.DriverId
            };
            await _TripDetailService.CreateAsync(tripDetail);

            line = await _LineService.GetLineWithDetailsAsync(line.Id);

            for (int x = 0; x < line.MidLines.Count; x++)
            {
                Trip trip = new()
                {
                    MidLineId = line.MidLines[x].Id,
                    ScheduleDate = Jobs.UpdateDateFormat(createLine.Date),
                    DepartureTime = createLine.Time[x],
                    ArrivalTime = createLine.Time[x + 1],
                    FareAmount = decimal.Parse(createLine.Fares[x]),
                    TripDetailId = tripDetail.Id
                };
                await _TripService.CreateAsync(trip);
            }


            return RedirectToAction("LineList");
        }

        public async Task<IActionResult> DeleteLine(int id)
        {
            var lineToBeDeleted = await _LineService.GetLineWithDetailsAsync(id);
            List<MidLine> midlinesToBeDeleted = lineToBeDeleted.MidLines;
            var tripsToBeDeleted = lineToBeDeleted.MidLines.Select(x => x.Trips[0]);
            var tripDetail = tripsToBeDeleted.First().TripDetail;
            do
            {
                await _MidlineService.DeleteAsync(midlinesToBeDeleted.First());
            } while (midlinesToBeDeleted.Count > 0);
            //do
            //{
            //    await _TripService.DeleteAsync(tripsToBeDeleted.First());
            //} while (midlinesToBeDeleted.Count > 0);
            await _LineService.DeleteAsync(lineToBeDeleted);
            await _TripDetailService.DeleteAsync(tripDetail);
            return RedirectToAction("LineList");
        }
        #endregion


        #region FlightActions
        public async Task<IActionResult> FlightList()
        {
            var buses = await _FlightService.GetAllAsync();
            return View(buses);
        }

        public IActionResult CreateFlight()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlight(FlightModel flightModel)
        {
            if (!ModelState.IsValid)
            {
                return View(flightModel);
            }
            var flight = new Flight()
            {
                Capacity = (int)flightModel.Capacity,
                HasWifi = flightModel.HasWifi,
                HasUSB = flightModel.HasUSB,
                HasSeatScreen = flightModel.HasSeatScreen
            };
            await _FlightService.CreateAsync(flight);

            return RedirectToAction("FlightList");
        }

        public async Task<IActionResult> UpdateFlight(int id)
        {
            var flight = await _FlightService.GetByIdAsync(id);
            FlightModel flightModel = new()
            {
                Id = flight.Id,
                Capacity = flight.Capacity,
                HasWifi = flight.HasWifi,
                HasUSB = flight.HasUSB,
                HasSeatScreen = flight.HasSeatScreen
            };
            return View(flightModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFlight(FlightModel flightModel)
        {
            var flight = await _FlightService.GetByIdAsync((int)flightModel.Id);
            flight.Capacity = (int)flightModel.Capacity;
            flight.HasUSB = flightModel.HasUSB;
            flight.HasWifi = flightModel.HasWifi;
            flight.HasSeatScreen = flightModel.HasSeatScreen;

            return RedirectToAction("FlightList");

        }
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var flight = await _FlightService.GetByIdAsync(id);
            await _FlightService.DeleteAsync(flight);

            return RedirectToAction("FlightList");

        }
        #endregion
    }
}

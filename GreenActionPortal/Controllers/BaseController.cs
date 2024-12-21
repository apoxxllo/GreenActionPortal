using GreenActionPortal.Models;
using GreenActionPortal.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GreenActionPortal.Controllers
{
    public class BaseController : Controller
    {
        public BaseRepository<User> _userRepo;
        public BaseRepository<AboutUsDetail> _aboutUsRepo;
        public BaseRepository<Activity> _activityRepo;
        public BaseRepository<GarbageCollectionDatum> _garbageCollectionDataRepo;
        public BaseRepository<GarbageCollectionSchedule> _garbageCollectionScheduleRepo;
        public BaseRepository<GarbageType> _garbageTypeRepo;
        public BaseRepository<Official> _officialRepo;
        public BaseRepository<Population> _populationRepo;
        public BaseRepository<Position> _positionRepo;

        public BaseController()
        {
            _aboutUsRepo = new BaseRepository<AboutUsDetail>();
            _activityRepo = new BaseRepository<Activity>();
            _userRepo = new BaseRepository<User>();
            _garbageCollectionDataRepo = new BaseRepository<GarbageCollectionDatum>();
            _garbageCollectionScheduleRepo = new BaseRepository<GarbageCollectionSchedule>();
            _officialRepo = new BaseRepository<Official>();
            _populationRepo = new BaseRepository<Population>();
            _positionRepo = new BaseRepository<Position>();
            _garbageTypeRepo = new BaseRepository<GarbageType>();
        }
    }
}

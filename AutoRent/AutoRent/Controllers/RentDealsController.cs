﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoRent.Models;

namespace AutoRent.Controllers
{
    public class RentDealsController : Controller
    {
        private AutoRentContext db = new AutoRentContext();

        private CarsController carsController = new CarsController();



        public ActionResult Index()
        {
            var rents = db.Rents.Include(r => r.car).
                Include(r => r.customer).Include(r => r.customerFavour);
            return View(rents.ToList());
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentDeal rentDeal = db.Rents.Find(id);
            if (rentDeal == null)
            {
                return HttpNotFound();
            }
            return View(rentDeal);
        }


        public ActionResult AddDeal(int? customerId, int? queryId, int? carId)
        {
            if (customerId == null | queryId == null | carId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var query = db.CustomerFavours.Find(queryId);

            var selectedCar = db.Cars.Find(carId);

            if (query == null || selectedCar == null)
            {
                return HttpNotFound();
            }

            var startingDate = query.rentStartDate;

            var daysForRent = query.rentDays;

            RentDeal anotherRentDeal = new RentDeal
            {
                CarID = carId,
                CustomerQueryID = queryId,
                CustomerID = customerId,
                car = db.Cars.Find(carId),
                customer = db.Customers.Find(customerId),
                customerFavour = db.CustomerFavours.Find(queryId),
                dateOfService = startingDate,
                dateOfReturn = startingDate.AddDays(daysForRent)
            };

            return View("Create", anotherRentDeal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CarID,CustomerID,CustomerQueryID,dateOfService,dateOfReturn")] RentDeal rentDeal)
        {
            if (ModelState.IsValid)
            {
                carsController.TakeCar(rentDeal.CarID);

                db.Rents.Add(rentDeal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rentDeal);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentDeal rentDeal = db.Rents.Find(id);
            if (rentDeal == null)
            {
                return HttpNotFound();
            }
            ViewBag.CarID = new SelectList(db.Cars, "ID", "brand", rentDeal.CarID);
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "firstName", rentDeal.CustomerID);
            ViewBag.CustomerQueryID = new SelectList(db.CustomerFavours, "ID", "favouriteBrand", rentDeal.CustomerQueryID);
            return View(rentDeal);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CarID,CustomerID,CustomerQueryID,dateOfService,dateOfReturn")] RentDeal rentDeal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rentDeal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CarID = new SelectList(db.Cars, "ID", "brand", rentDeal.CarID);
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "firstName", rentDeal.CustomerID);
            ViewBag.CustomerQueryID = new SelectList(db.CustomerFavours, "ID", "favouriteBrand", rentDeal.CustomerQueryID);
            return View(rentDeal);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentDeal rentDeal = db.Rents.Find(id);
            if (rentDeal == null)
            {
                return HttpNotFound();
            }
            return View(rentDeal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RentDeal rentDeal = db.Rents.Find(id);
            db.Rents.Remove(rentDeal);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

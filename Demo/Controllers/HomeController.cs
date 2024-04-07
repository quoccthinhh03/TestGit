using Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        dataDataContext db = new dataDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachHoGiaDinh()
        {
            var danhSachHoGiaDinh = db.HocGiaDinhs.ToList();
            return View(danhSachHoGiaDinh);
        }
        public ActionResult DanhSachNguoiCuTru()
        {
            var danhSachNguoiCuTru = db.NguoiCuTrus.ToList();
            return View(danhSachNguoiCuTru);
        }

        public ActionResult ThemMoiDongGopPhi()
        {
            return View();
        }

        // POST: Home/ThemMoiDongGopPhi
        [HttpPost]
        public ActionResult ThemMoiDongGopPhi(DongGopPhi dongGopPhi)
        {
            if (ModelState.IsValid)
            {
                db.DongGopPhis.InsertOnSubmit(dongGopPhi);
                db.SubmitChanges();

                return RedirectToAction("Index", "Home");
            }
            return View(dongGopPhi);
        }

        public ActionResult TongSoTienDaThu()
        {
            var tongSoTienDaThu = db.DongGopPhis.Sum(d => d.SoTien);
            return View(tongSoTienDaThu);
        }

        public ActionResult ChiTietKhoanTienDaNop(int id)
        {
            var chiTietKhoanTienDaNop = db.DongGopPhis.Where(d => d.MaHocGiaDinh == id).ToList();
            return View(chiTietKhoanTienDaNop);
        }


        public ActionResult Phivesinh()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Phivesinh(int? maHocGiaDinh, PhiVeSinh model)
        {
            if (ModelState.IsValid && maHocGiaDinh != null)
            {
                // Tìm thông tin hộ gia đình theo mã
                var hocGiaDinh = db.HocGiaDinhs.FirstOrDefault(h => h.MaHocGiaDinh == maHocGiaDinh);

                if (hocGiaDinh != null)
                {
                    // Tính tổng số tiền phí vệ sinh
                    int totalPeople = hocGiaDinh.TongSoNguoiTrongGiaDinh ?? 0;
                    decimal totalFee = totalPeople * 6000;

                    // Lưu thông tin phí vệ sinh vào cơ sở dữ liệu
                    var phiVeSinh = new PhiVeSinh
                    {
                        MaHocGiaDinh = maHocGiaDinh,
                        SoTien = totalFee,
                        Thang = model.Thang,
                        Nam = model.Nam
                    };

                    db.PhiVeSinhs.InsertOnSubmit(phiVeSinh);
                    db.SubmitChanges();

                    // Chuyển hướng về trang chính sau khi xử lý
                    return RedirectToAction("Index");
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy thông tin hộ gia đình
                    ModelState.AddModelError("", "Không tìm thấy thông tin hộ gia đình");
                }
            }

            // Nếu dữ liệu không hợp lệ hoặc mã hộ gia đình không hợp lệ, hiển thị lại view với model
            return View(model);
        }

        public ActionResult PhiVeSinhCuaHoGiaDinh()
        {
            // Lấy danh sách phí vệ sinh của các hộ gia đình từ cơ sở dữ liệu
            var phiVeSinhCuaHoGiaDinh = db.PhiVeSinhs.ToList();
            return View(phiVeSinhCuaHoGiaDinh);
        }

        public ActionResult ThemMoiHocGiaDinh()
        {
            return View();
        }

        // Xử lý việc thêm mới hộ gia đình
        [HttpPost]
        public ActionResult ThemMoiHocGiaDinh(HocGiaDinh hocGiaDinh)
        {
            if (ModelState.IsValid)
            {
                db.HocGiaDinhs.InsertOnSubmit(hocGiaDinh);
                db.SubmitChanges();

                return RedirectToAction("Index", "Home");
            }
            return View(hocGiaDinh);
        }

        public ActionResult ThemMoiNguoiCuTru()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ThemMoiNguoiCuTru(FormCollection form)
        {
            // Lấy số lượng người từ form
            int.TryParse(form["SoLuongNguoi"], out int soLuongNguoi);

            // Lấy mã hộ gia đình từ form
            int.TryParse(form["MaHocGiaDinh"], out int maHocGiaDinh);

            // Lưu thông tin từ form vào cơ sở dữ liệu
            for (int i = 1; i <= soLuongNguoi; i++)
            {
                var nguoiCuTru = new NguoiCuTru
                {
                    MaHocGiaDinh = maHocGiaDinh,
                    HoTen = form["HoTen" + i],
                    Tuoi = int.Parse(form["Tuoi" + i]),
                    GioiTinh = form["GioiTinh" + i],
                    MoiQuanHe = form["MoiQuanHe" + i]
                };

               
                 db.NguoiCuTrus.InsertOnSubmit(nguoiCuTru);
                 db.SubmitChanges();
            }

            // Sau khi lưu xong, chuyển hướng về trang danh sách người cư trú
            return RedirectToAction("DanhSachNguoiCuTru");
        }






    }
}
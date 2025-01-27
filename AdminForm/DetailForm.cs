﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BLL;
using DTO;


namespace AdminForm
{
    public partial class DetailForm : Form
    {

        public int idMonOfMainForm { get; set; }
        public delegate void Mydel(int idDanhMuc, string tenMon);
        public Mydel actionAfterOk { get; set; }
        public DetailForm(int idMon)
        {
            InitializeComponent();
            this.idMonOfMainForm = idMon;
            setCBB();
            setUI();
        }


        #region support menthod
        private void ShowAnh(int idAnh)
        {
            AnhMinhHoa anh = BusinessLogicLayer.Instance.GetIdAnhByIdAnh(idAnh);
            picHinhAnh.Image = Image.FromStream(BusinessLogicLayer.Instance.GetByteValuesOfAnh(anh.IdAnh), true);
            picHinhAnh.Image.Tag = anh.IdAnh;
        }
        private void setUI()
        {
            if (this.idMonOfMainForm != 0)
            {
                
                Mon mon = BusinessLogicLayer.Instance.GetMonByIdMon(this.idMonOfMainForm);
                txtIdMon.Text = mon.IdMon +"";
                txtTenMon.Text = mon.TenMon;
                txtGiaTien.Text = mon.GiaTien + "";
                txtSoLanGoiMon.Text = mon.SoLanGoiMon + "";
                ShowAnh(mon.IdAnh);

                //  cboLSH.SelectedIndex = s.ID_Lop - 1 ;
                int index = 0;
                foreach (CBBItem i in cboDanhMuc.Items)
                {
                    if (mon.IdDanhMuc == i.Value) break;
                    index++;
                }
                cboDanhMuc.SelectedIndex = index;

                txtIdMon.Enabled = false;
                btnOk.Text = "Update";
            }
            else
            {
                txtIdMon.Text = BusinessLogicLayer.Instance.GetMaxIdMon() + 1 + "";
                if (picHinhAnh.Image == null) ShowAnh(0);
                txtIdMon.Enabled = false;
                btnOk.Text = "Add";
            }

        }
        private void setCBB()
        {
            BusinessLogicLayer.Instance.SetCbbDetailForm(cboDanhMuc);
        }
        private bool ValidValues()
        {
            int intValue;
            if(!string.IsNullOrEmpty(txtTenMon.Text) && !string.IsNullOrEmpty(txtSoLanGoiMon.Text) && !string.IsNullOrEmpty(txtGiaTien.Text)
                && int.TryParse(txtGiaTien.Text, out int n) && int.TryParse(txtSoLanGoiMon.Text, out int m))
            {
                return true;
            }
            return false;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ValidValues())
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin!");
                return;
            }
            Mon mon = new Mon
            {
                IdMon = int.Parse(txtIdMon.Text),
                TenMon = txtTenMon.Text,
                GiaTien = int.Parse(txtGiaTien.Text),
                SoLanGoiMon = int.Parse(txtSoLanGoiMon.Text),
                IdDanhMuc = ((CBBItem)cboDanhMuc.SelectedItem).Value,
                IdAnh = (int)picHinhAnh.Image.Tag
            };

            if (BusinessLogicLayer.Instance.ExcuteDB_BLL(mon, this.idMonOfMainForm))
            {
                DialogResult = DialogResult.OK;
            }
            else DialogResult = DialogResult.Cancel;

            actionAfterOk(0, null);
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "JPG (*.jpg)|*.jpg|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    picHinhAnh.Image = Image.FromFile(openFileDialog.FileName);
                    string teAnh = openFileDialog.FileName.Substring(openFileDialog.FileName.LastIndexOf("\\"));

                    FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] anh = br.ReadBytes((int)fs.Length);
                    BusinessLogicLayer.Instance.ThemAnhVaoDb(teAnh, anh);
                    picHinhAnh.Image.Tag = BusinessLogicLayer.Instance.GetMaxIdAnh();

                }
            }
        }

        private void btnPull_Click(object sender, EventArgs e)
        {
            ImagesForm f = new ImagesForm();
            f.afterOk += new ImagesForm.MyDel(ShowAnh);
            if(f.ShowDialog() == DialogResult.OK)
            {
                ShowAnh((int)picHinhAnh.Image.Tag);
            }
        }
    }
}

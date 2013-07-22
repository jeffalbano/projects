﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;

namespace BrightVision.Reporting.UI
{
    public partial class XtraReportDefault : DevExpress.XtraReports.UI.XtraReport
    {
        private TopMarginBand topMarginBand1;
        private DetailBand detailBand1;
        private BottomMarginBand bottomMarginBand1;
    
        public XtraReportDefault()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.detailBand1 = new DevExpress.XtraReports.UI.DetailBand();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // detailBand1
            // 
            this.detailBand1.Name = "detailBand1";
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // XtraReportDefault3
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.topMarginBand1,
            this.detailBand1,
            this.bottomMarginBand1});
            this.Version = "11.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }

        public static XtraReportDefault CreateXtraReport(DataSet dataset)
        {
            XtraReportDefault xtrareport = new XtraReportDefault();

            #region Account Level
            var accountBand = xtrareport.detailBand1;
            var xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            accountBand.HeightF = 116.6667F;
            accountBand.Name = "Detail";
            accountBand.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            accountBand.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;

            // 
            // xrLine1
            // 
            xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            xrLine1.Name = "xrLine1";
            xrLine1.SizeF = new System.Drawing.SizeF(650F, 23F);

            //accountBand.Controls.Add(xrLine1);

            var columns = dataset.Tables["Account"].Columns;
            float height = 35;
            foreach (DataColumn column in columns) {
                if (column.ColumnName == "Id") continue;
                var xrLabelName = new DevExpress.XtraReports.UI.XRLabel();
                xrLabelName.LocationFloat = new DevExpress.Utils.PointFloat(33.75017F, height);
                xrLabelName.Name = "xrLabel13";
                 
                xrLabelName.SizeF = new System.Drawing.SizeF(200F, 23F);
                xrLabelName.StylePriority.UseTextAlignment = false;
                xrLabelName.Text = column.ColumnName+":";
                xrLabelName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;

                var xrLabelValues = new DevExpress.XtraReports.UI.XRLabel();
                xrLabelValues.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
                new DevExpress.XtraReports.UI.XRBinding("Text", null, "Account."+column.ColumnName)});
                xrLabelValues.LocationFloat = new DevExpress.Utils.PointFloat(243.7502F, height);
                xrLabelValues.Name = "xrLabel49";
                xrLabelValues.CanShrink = true;
                xrLabelValues.CanGrow = true;
                xrLabelValues.Multiline = true;
                xrLabelValues.SizeF = new System.Drawing.SizeF(350F, 23F);
                xrLabelValues.Text = "xrLabel49";
                height += 35;
                accountBand.Controls.Add(xrLabelName);
                accountBand.Controls.Add(xrLabelValues);
            }
            height += 35;
            var xrLine4 = new DevExpress.XtraReports.UI.XRLine();
            xrLine4.LocationFloat = new DevExpress.Utils.PointFloat(0F, height);
            xrLine4.Name = "xrLine4";
            xrLine4.SizeF = new System.Drawing.SizeF(650F, 23F);
            accountBand.Controls.Add(xrLine4);

            var xrPageBreak2 = new DevExpress.XtraReports.UI.XRPageBreak();
            xrPageBreak2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            xrPageBreak2.Name = "xrPageBreak2";
             accountBand.Controls.Add(xrPageBreak2);
           
            #endregion
            #region Contact level

            var contactDetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            var contactDetailBand = new DevExpress.XtraReports.UI.DetailBand();
            contactDetailBand.HeightF = 116.6667F;
            contactDetailBand.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            contactDetailBand.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            contactDetailBand.Name = "Detail";
            contactDetailReport.DataSource = dataset;
            contactDetailReport.DataMember = "account.account_contact";
            contactDetailReport.Level = 1;
            contactDetailReport.Name = "DetailReport3";

            var cColumns = dataset.Tables["Contact"].Columns;
            height = 35;
            foreach (DataColumn column in cColumns)
            {
                if (column.ColumnName == "AccountId" || column.ColumnName == "Id") continue;
                var xrLabelName = new DevExpress.XtraReports.UI.XRLabel();
                xrLabelName.LocationFloat = new DevExpress.Utils.PointFloat(33.75017F, height);
                xrLabelName.Name = "xrLabel13";

                xrLabelName.SizeF = new System.Drawing.SizeF(200F, 23F);
                xrLabelName.StylePriority.UseTextAlignment = false;
                xrLabelName.Text = column.ColumnName+":";
                xrLabelName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;

                var xrLabelValues = new DevExpress.XtraReports.UI.XRLabel();
                xrLabelValues.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
                new DevExpress.XtraReports.UI.XRBinding("Text", null, "account.account_contact."+column.ColumnName)});
                xrLabelValues.LocationFloat = new DevExpress.Utils.PointFloat(243.7502F, height);
                xrLabelValues.Name = "xrLabel49";
                xrLabelValues.CanShrink = true;
                xrLabelValues.CanGrow = true;
                xrLabelValues.Multiline = true;
                xrLabelValues.SizeF = new System.Drawing.SizeF(350F, 23F);
                xrLabelValues.Text = "xrLabel49";
                height += 35;
                contactDetailBand.Controls.Add(xrLabelName);
                contactDetailBand.Controls.Add(xrLabelValues);
            }
            height += 35;
            xrLine4 = new DevExpress.XtraReports.UI.XRLine();
            xrLine4.LocationFloat = new DevExpress.Utils.PointFloat(0F, height);
            xrLine4.Name = "xrLine4";
            xrLine4.SizeF = new System.Drawing.SizeF(650F, 23F);
            contactDetailBand.HeightF = height+35;
            contactDetailBand.Controls.Add(xrLine4);
            contactDetailReport.Bands.Add(contactDetailBand);

            xtrareport.Bands.AddRange(new Band[] { accountBand, contactDetailReport });
            //xtrareport.Bands.Add(accountBand);
            //xtrareport.Bands.Add(contactDetailReport);

            #endregion

            xtrareport.DataSource = dataset;
            return xtrareport;
        }
        public static XtraReportDefaultTemplate CreateXtraReportDefaultTemplate(ReportDataSet reportDataset) {
            XtraReportDefaultTemplate rdt = new XtraReportDefaultTemplate();

            rdt.DataSource = reportDataset;
            return rdt;
        }
    }
}
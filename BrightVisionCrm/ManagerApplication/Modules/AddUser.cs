﻿
#region Namespace
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using System.Data.Objects;
using System.Data.Objects.DataClasses;
using BrightVision.Model;
using BrightVision.Common.Utilities;
using BrightVision.Common.Business;
using ManagerApplication.Business;
using DevExpress.XtraEditors.Controls;
#endregion

namespace ManagerApplication.Modules
{
    public partial class AddUser : DevExpress.XtraEditors.XtraUserControl
    {
        #region Enumerations
        public enum SaveType
        {
            SaveTypeAdd,
            SaveTypeEdit
        }

        public enum eUserType
        {
            InternalUser,
            CustomerUser
        }
        #endregion

        #region Public Members
        public ManageInternalUser objInternalUserControl = null; //object to handle access on the user parent form controls and functions
        public ManageCustomerUser objCustomerUserControl = null; //object to handle access on the user parent form controls and functions
        public int UserId { get; set; }
        public bool IsNew { get; set; }
        public eUserType UserType { get; set; }
        #endregion
        
        #region Private Members
        private BrightPlatformEntities m_objBrightPlatformEntity = new BrightPlatformEntities(UserSession.EntityConnection);
        private ObjectUser.UserInstance m_objUser = null;
        private bool m_hasChangedPassword = false;
        private SaveType m_eSaveType = SaveType.SaveTypeAdd;
        private string m_MessageBoxCaption = "Manager Application - Users";
        #endregion

        #region Contructors
        public AddUser()
        {
            this.Visible = false;
            InitializeComponent();
            this.Visible = true;
        }

        /// <summary>
        /// Constructor to initialize save type and the user data object to edit
        /// </summary>
        public AddUser(SaveType eSaveType, ObjectUser.UserInstance objUser)
        {
            this.Visible = false;
            InitializeComponent();
            m_objUser = objUser;
            m_eSaveType = eSaveType;
            this.Visible = true;
            this.layoutControlItemSIP.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }

        /// <summary>
        /// Constructor to initialize save type and the user data object to edit
        /// </summary>
        public AddUser(SaveType eSaveType, ObjectUser.UserInstance objUser, bool EditCredentialsOnly)
        {
            this.Visible = false;
            InitializeComponent();
            m_objUser = objUser;
            m_eSaveType = eSaveType;

            if (EditCredentialsOnly)
            {
                txtFullname.Enabled = false;
                txtTitle.Enabled = false;
                cboManager.Enabled = false;
                txtSite.Enabled = false;
                txtPhone.Enabled = false;
                txtMobile.Enabled = false;
                txtEmail.Enabled = false;
                chkActive.Enabled = false;
            }

            this.Visible = true;
        }
        #endregion

        #region Object Control Events
        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (UserType == eUserType.InternalUser && (txtUsername.Text.Length < 1 || txtPassword.Text.Length < 1)) {
                MessageBox.Show("Not a valid username/password.", m_MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.SaveUser(IsNew);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            m_hasChangedPassword = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the module
        /// </summary>
        public void InitializeModule()
        {
            this.LoadComboData();

            if (UserType == eUserType.CustomerUser)
            {
                this.PopulateCustomers();
                cboCustomer.Enabled = true;
            }

            if (m_eSaveType == SaveType.SaveTypeEdit)
            {
                this.InitControls();
                this.DisplayUserInformation();
            }
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Logic to save user record
        /// </summary>
        private void SaveUser(bool IsNew)
        {
            if (!ValidateEntries())
                return;

            WaitDialog.Show(ParentForm, "Saving user data...");
            ObjectUser.UserInstance objParams = new ObjectUser.UserInstance() {
                id = UserId,
                full_name = txtFullname.Text,
                title = txtTitle.Text,
                reports_to = (int) cboManager.EditValue,
                site = txtSite.Text,
                active = chkActive.Checked,
                phone = txtPhone.Text,
                mobile = txtMobile.Text,
                email = txtEmail.Text,
                username = txtUsername.Text
            };

            if (IsNew) {
                if (txtPassword.Text.Length < 1)
                    objParams.password = HashUtility.GetHashPassword("1234");
                else
                    objParams.password = HashUtility.GetHashPassword(txtPassword.Text);
            }
            else {
                if (m_hasChangedPassword)
                    objParams.password = HashUtility.GetHashPassword(txtPassword.Text);
            }

            if (Convert.ToInt32(cboSIP.EditValue) > 0)
                objParams.sip_id = (int)(cboSIP.EditValue);

            if (UserType == eUserType.InternalUser)
                objParams.internal_user = true;
            else
                objParams.internal_user = false;
            
            UserId = ObjectUser.SaveUser(IsNew, objParams, m_hasChangedPassword);
            if (UserType == eUserType.InternalUser)
                objInternalUserControl.PopulateUserGrid();
            else {
                ObjectUser.SaveUserCustomer(UserId, (int)cboCustomer.EditValue);
                objCustomerUserControl.PopulateUserGrid(objParams.full_name);
            }
            WaitDialog.Close();
            this.ParentForm.Close();
        }

        /// <summary>
        /// Provides the validation logic when adding/editing a record
        /// </summary>
        private bool ValidateEntries()
        {
            if (txtFullname.Text.Trim().Count() < 1)
            {
                this.DisplayValidationError("first name");
                return false;
            }
            //else if (txtPhone.Text.Trim().Count() > 0)
            //    /* implement validation here */
            //    ;

            //else if (txtMobile.Text.Trim().Count() > 0)
            //    /* implement validation here */
            //    ;

            else if (txtEmail.Text.Trim().Count() > 0)
            {
                if (!ValidationUtility.IsEmail(txtEmail.Text))
                {
                    this.DisplayValidationError("email address");
                    return false;
                }
            }
            else if (txtUsername.Text.Trim().Count() < 1)
            {
                this.DisplayValidationError("username");
                return false;
            }
            else if (txtPassword.Text.Trim().Count() < 1)
            {
                this.DisplayValidationError("password");
                return false;
            }
            else if (this.UserExist()) // verify that firstname and lastname already exists
            {
                MessageBox.Show("User already exists!", m_MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else if (UserType == eUserType.CustomerUser && Convert.ToInt32(cboCustomer.EditValue) == 0)
            {
                MessageBox.Show("Please assign a customer for this user.", m_MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Provides the validation logic for existing users when adding new data
        /// </summary>
        private bool UserExist()
        {
            bool _IsInternalUser = false;
            if (UserType == eUserType.InternalUser)
                _IsInternalUser = true;
            else
                _IsInternalUser = false;
            return ObjectUser.UserExists(IsNew, txtFullname.Text, _IsInternalUser);
        }

        /// <summary>
        /// Simple method just to display the error message as per string parameter specified --
        /// </summary>
        private void DisplayValidationError(string strFieldName)
        {
            MessageBox.Show("Invalid " + strFieldName, m_MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Initializes the user form object members
        /// </summary>
        private void InitControls()
        {
            txtFullname.Text = "";
            txtTitle.Text = "";
            txtSite.Text = "";
            chkActive.Checked = false;
            txtPhone.Text = "";
            txtMobile.Text = "";
            txtEmail.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
        }

        /// <summary>
        /// Displays the user information in the user form when editing the current selected record
        /// </summary>
        private void DisplayUserInformation()
        {
            this.LoadComboData();
            var objEntityUser = m_objBrightPlatformEntity.users.Where(objField => objField.id == m_objUser.id).SingleOrDefault();
            UserId = 0;

            txtFullname.Text = objEntityUser.fullname;
            txtTitle.Text = objEntityUser.title;
            cboManager.EditValue = m_objUser.reports_to;
            txtSite.Text = objEntityUser.site;
            chkActive.Checked = objEntityUser.disabled == 0? true : false;
            txtPhone.Text = objEntityUser.phone1;
            txtMobile.Text = objEntityUser.mobile_no;
            txtEmail.Text = objEntityUser.email;
            txtUsername.Text = objEntityUser.username;
            txtPassword.Text = objEntityUser.password;
            cboSIP.EditValue = objEntityUser.sip_id;
            //chkInternalUser.Checked = objEntityUser.internal_user ? true : false;
            UserId = objEntityUser.id;
            cboCustomer.EditValue = m_objUser.customer_id;
        }

        /// <summary>
        /// Populates the manager combo box on the user form
        /// </summary>
        private void PopulateManagers()
        {
            cboManager.Properties.Columns.Clear();
            cboManager.Properties.DataSource = null;
            cboManager.Properties.DataSource = ObjectUser.GetManagers().Execute(MergeOption.AppendOnly);
            cboManager.Properties.DisplayMember = "manager_name";
            cboManager.Properties.ValueMember = "id";
            cboManager.Properties.Columns.Add(new LookUpColumnInfo("manager_name"));
            cboManager.ItemIndex = 0;
        }
        /// <summary>
        /// Populates the manager combo box on the user form
        /// </summary>
        private void PopulateSIP()
        {
            cboSIP.Properties.Columns.Clear();
            cboSIP.Properties.DataSource = null;
            //cboSIP.Properties.DataSource = ObjectUser.GetSIPAccounts().Execute(MergeOption.AppendOnly);
            cboSIP.Properties.DataSource = ObjectUser.GetSIPAccounts();
            cboSIP.Properties.DisplayMember = "display_name";
            cboSIP.Properties.ValueMember = "id";
            cboSIP.Properties.Columns.Add(new LookUpColumnInfo("display_name"));
            cboSIP.Properties.ShowHeader = false;
            cboSIP.ItemIndex = 0;
            cboSIP.EditValue = -1;
        }
        /// <summary>
        /// Populates the customer combo box on the user form
        /// </summary>
        private void PopulateCustomers()
        {
            cboCustomer.Properties.DataSource = null;
            cboCustomer.Properties.DataSource = ObjectCustomer.GetCustomers(ObjectCustomer.eViewType.ComboListView).Execute(MergeOption.AppendOnly);
            cboCustomer.Properties.DisplayMember = "customer_name";
            cboCustomer.Properties.ValueMember = "id";
            cboCustomer.Properties.Columns.Add(new LookUpColumnInfo("customer_name"));
            cboCustomer.ItemIndex = 0;
        }

        /// <summary>
        /// Populates the customer and manager combo box on the user form
        /// </summary>
        private void LoadComboData()
        {
            this.PopulateManagers();
            this.PopulateSIP();
        }
        #endregion
    }
}

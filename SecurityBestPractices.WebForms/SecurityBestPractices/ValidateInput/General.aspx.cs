using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices.UsingAntiForgeryToken {
    public partial class GeneralEditForm : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            // Init Value 
            if(!IsPostBack) {
                spinEdit.Value = 10000; // from DataSource

                someEdit.Value = 55;
            }

            // Init OnValidation
            //spinEdit.Validation += ASPxValidationHelper.Validation;
            //someEdit.Validation += ASPxValidationHelper.Validation;
        }

        protected void UpdateButton_Click(object sender, EventArgs e) {
            ASPxEdit.ValidateEditorsInContainer(Page); // TODO: Check and correct documenation 
            if(ASPxEdit.AreEditorsValid(Page)) {
                UpdateStatusLabel.Text = string.Format("Submitted value: {0}", spinEdit.Value);
            } else {
                UpdateStatusLabel.Text = "Editors are not valid!";
            }
        }

        protected void spinEdit_Validation(object sender, ValidationEventArgs e) {
            if((sender as ASPxSpinEdit).Number == 5) {
                e.IsValid = false;
                e.ErrorText = "5 is not acceptable";
            }
        }
    }

    public static class ASPxValidationHelper {
        public static void Validation(object sender, ValidationEventArgs e) {
            if(!e.IsValid) return;

            // TextEdit
            if(sender is ASPxTextBoxBase) {
                var textEdit = (ASPxTextBoxBase)sender;
                if(textEdit.Value != null) {
                    if(textEdit.MaxLength != 0)
                        e.IsValid = textEdit.Text.Length <= textEdit.MaxLength;
                        if(!e.IsValid)
                            e.ErrorText = "Text.Length > MaxLength!";
                }
            }

            // SpinEdit
            if(sender is ASPxSpinEdit) {
                var spinEdit = (ASPxSpinEdit)sender;
                if(spinEdit.Value != null) {
                    if(spinEdit.MaxValue != 0 || spinEdit.MinValue != 0) {
                        decimal decimalValue = (decimal)spinEdit.Value;
                        e.IsValid = decimalValue >= spinEdit.MinValue && decimalValue <= spinEdit.MaxValue;
                        if(!e.IsValid)
                            e.ErrorText = "Out of range!";
                    }
                }
            }
        }
    }
}
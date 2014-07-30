/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
using System.Xml.Serialization;
using Dover.Framework.Monad;
using System;

namespace Dover.Framework.Model.SAP
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "BOM", Namespace = "", IsNullable = false)]
    public partial class DocumentsBOM : IBOM
    {

        private IBO[] boField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BO", Type=typeof(DocumentsBOMBO))]
        public IBO[] BO
        {
            get
            {
                return this.boField;
            }
            set
            {
                this.boField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DocumentsBOMBO : IBO
    {
        private BOMBOAdmInfo admInfoField;

        private BOMBOQueryParams queryParamsField;

        private DocumentHeader[] documentsField;

        private DocumentApprovalRequestsField[] document_ApprovalRequestsField;

        private DocumentLine[] document_LinesField;

        private LineTaxJurisdictionsField[] lineTaxJurisdictionsField;

        private SerialNumbersField[] serialNumbersField;

        private BatchNumbersField[] batchNumbersField;

        private DocumentLinesBinAllocationsField[] documentLinesBinAllocationsField;

        private TaxExtensionField[] taxExtensionField;

        private AddressExtensionField[] addressExtensionField;

        internal override string[] GetKey()
        {
            return new string[] { "DocEntry" };
        }

        [XmlIgnore]
        public SAPbobsCOM.BoObjectTypes BOType { get; set; }

        internal override SAPbobsCOM.BoObjectTypes GetBOType()
        {
            return BOType;
        }

        internal override System.Type GetBOClassType()
        {
            return typeof(SAPbobsCOM.Documents);
        }

        internal override string GetName()
        {
            switch (BOType)
            {

                case SAPbobsCOM.BoObjectTypes.oInvoices:
                    return Messages.oInvoices;
                case SAPbobsCOM.BoObjectTypes.oCreditNotes:
                    return Messages.oCreditNotes;
                case SAPbobsCOM.BoObjectTypes.oDeliveryNotes:
                    return Messages.oDeliveryNotes;
                case SAPbobsCOM.BoObjectTypes.oReturns:
                    return Messages.oReturns;
                case SAPbobsCOM.BoObjectTypes.oOrders:
                    return Messages.oOrders;
                case SAPbobsCOM.BoObjectTypes.oPurchaseInvoices:
                    return Messages.oPurchaseInvoices;
                case SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes:
                    return Messages.oPurchaseCreditNotes;
                case SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes:
                    return Messages.oPurchaseDelivertNotes;
                case SAPbobsCOM.BoObjectTypes.oPurchaseReturns:
                    return Messages.oPurchaseReturns;
                case SAPbobsCOM.BoObjectTypes.oPurchaseOrders:
                    return Messages.oPurchaseOrders;
                case SAPbobsCOM.BoObjectTypes.oQuotations:
                    return Messages.oQuotations;
                case SAPbobsCOM.BoObjectTypes.oInventoryGenExit:
                    return Messages.oInventoryGenExit;
                case SAPbobsCOM.BoObjectTypes.oInventoryGenEntry:
                    return Messages.oInventoryGenEntry;
                default:
                    return Messages.DefaultDocument;
            }
        }

        internal override string GetFormattedKey()
        {
            return "[" + Documents
                .With(x => x[0])
                .Return(x => x.DocEntry, 0) + "]";
        }

        internal override string GetFormattedDescription()
        {
            return "[" + Documents
                .With(x => x[0])
                .Return(x => x.DocEntry, 0) + "]";
        }

        /// <remarks/>
        public BOMBOAdmInfo AdmInfo
        {
            get
            {
                return this.admInfoField;
            }
            set
            {
                this.admInfoField = value;
            }
        }

        /// <remarks/>
        public BOMBOQueryParams QueryParams
        {
            get
            {
                return this.queryParamsField;
            }
            set
            {
                this.queryParamsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false, Namespace="")]
        public DocumentHeader[] Documents
        {
            get
            {
                return this.documentsField;
            }
            set
            {
                this.documentsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public DocumentApprovalRequestsField[] Document_ApprovalRequests
        {
            get
            {
                return this.document_ApprovalRequestsField;
            }
            set
            {
                this.document_ApprovalRequestsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public DocumentLine[] Document_Lines
        {
            get
            {
                return this.document_LinesField;
            }
            set
            {
                this.document_LinesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public LineTaxJurisdictionsField[] LineTaxJurisdictions
        {
            get
            {
                return this.lineTaxJurisdictionsField;
            }
            set
            {
                this.lineTaxJurisdictionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public SerialNumbersField[] SerialNumbers
        {
            get
            {
                return this.serialNumbersField;
            }
            set
            {
                this.serialNumbersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public BatchNumbersField[] BatchNumbers
        {
            get
            {
                return this.batchNumbersField;
            }
            set
            {
                this.batchNumbersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public DocumentLinesBinAllocationsField[] DocumentLinesBinAllocations
        {
            get
            {
                return this.documentLinesBinAllocationsField;
            }
            set
            {
                this.documentLinesBinAllocationsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public TaxExtensionField[] TaxExtension
        {
            get
            {
                return this.taxExtensionField;
            }
            set
            {
                this.taxExtensionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public AddressExtensionField[] AddressExtension
        {
            get
            {
                return this.addressExtensionField;
            }
            set
            {
                this.addressExtensionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DocumentHeader
    {

        private long docEntryField;

        private bool docEntryFieldSpecified;

        private long docNumField;

        private bool docNumFieldSpecified;

        private string docTypeField;

        private string handWrittenField;

        private string printedField;

        private DateTime docDateField;

        private DateTime docDueDateField;

        private string cardCodeField;

        private string cardNameField;

        private string addressField;

        private string numAtCardField;

        private double docTotalField;

        private bool docTotalFieldSpecified;

        private long attachmentEntryField;

        private bool attachmentEntryFieldSpecified;

        private string docCurrencyField;

        private double docRateField;

        private bool docRateFieldSpecified;

        private string reference1Field;

        private string reference2Field;

        private string commentsField;

        private string journalMemoField;

        private long paymentGroupCodeField;

        private bool paymentGroupCodeFieldSpecified;

        private long docTimeField;

        private bool docTimeFieldSpecified;

        private long salesPersonCodeField;

        private bool salesPersonCodeFieldSpecified;

        private long transportationCodeField;

        private bool transportationCodeFieldSpecified;

        private string confirmedField;

        private long importFileNumField;

        private bool importFileNumFieldSpecified;

        private string summeryTypeField;

        private long contactPersonCodeField;

        private bool contactPersonCodeFieldSpecified;

        private string showSCNField;

        private long seriesField;

        private bool seriesFieldSpecified;

        private DateTime taxDateField;

        private string partialSupplyField;

        private string docObjectCodeField;

        private string shipToCodeField;

        private string indicatorField;

        private string federalTaxIDField;

        private double discountPercentField;

        private bool discountPercentFieldSpecified;

        private string paymentReferenceField;

        private double docTotalFcField;

        private bool docTotalFcFieldSpecified;

        private long form1099Field;

        private bool form1099FieldSpecified;

        private string box1099Field;

        private string revisionPoField;

        private string requriedDateField;

        private string cancelDateField;

        private string blockDunningField;

        private string pickField;

        private string paymentMethodField;

        private string paymentBlockField;

        private long paymentBlockEntryField;

        private bool paymentBlockEntryFieldSpecified;

        private string centralBankIndicatorField;

        private string maximumCashDiscountField;

        private string projectField;

        private string exemptionValidityDateFromField;

        private string exemptionValidityDateToField;

        private string wareHouseUpdateTypeField;

        private string roundingField;

        private string externalCorrectedDocNumField;

        private long internalCorrectedDocNumField;

        private bool internalCorrectedDocNumFieldSpecified;

        private string deferredTaxField;

        private string taxExemptionLetterNumField;

        private string agentCodeField;

        private long numberOfInstallmentsField;

        private bool numberOfInstallmentsFieldSpecified;

        private string applyTaxOnFirstInstallmentField;

        private string vatDateField;

        private long documentsOwnerField;

        private bool documentsOwnerFieldSpecified;

        private string folioPrefixStringField;

        private long folioNumberField;

        private bool folioNumberFieldSpecified;

        private string documentSubTypeField;

        private string bPChannelCodeField;

        private long bPChannelContactField;

        private bool bPChannelContactFieldSpecified;

        private string address2Field;

        private string payToCodeField;

        private string manualNumberField;

        private string useShpdGoodsActField;

        private string isPayToBankField;

        private string payToBankCountryField;

        private string payToBankCodeField;

        private string payToBankAccountNoField;

        private string payToBankBranchField;

        private long bPL_IDAssignedToInvoiceField;

        private bool bPL_IDAssignedToInvoiceFieldSpecified;

        private double downPaymentField;

        private bool downPaymentFieldSpecified;

        private string reserveInvoiceField;

        private long languageCodeField;

        private bool languageCodeFieldSpecified;

        private string trackingNumberField;

        private string pickRemarkField;

        private string closingDateField;

        private long sequenceCodeField;

        private bool sequenceCodeFieldSpecified;

        private long sequenceSerialField;

        private bool sequenceSerialFieldSpecified;

        private string seriesStringField;

        private string subSeriesStringField;

        private string sequenceModelField;

        private string useCorrectionVATGroupField;

        private double downPaymentAmountField;

        private bool downPaymentAmountFieldSpecified;

        private double downPaymentPercentageField;

        private bool downPaymentPercentageFieldSpecified;

        private string downPaymentTypeField;

        private double downPaymentAmountSCField;

        private bool downPaymentAmountSCFieldSpecified;

        private double downPaymentAmountFCField;

        private bool downPaymentAmountFCFieldSpecified;

        private double vatPercentField;

        private bool vatPercentFieldSpecified;

        private double serviceGrossProfitPercentField;

        private bool serviceGrossProfitPercentFieldSpecified;

        private string openingRemarksField;

        private string closingRemarksField;

        private double roundingDiffAmountField;

        private bool roundingDiffAmountFieldSpecified;

        private string controlAccountField;

        private string insuranceOperation347Field;

        private string archiveNonremovableSalesQuotationField;

        private long gTSCheckerField;

        private bool gTSCheckerFieldSpecified;

        private long gTSPayeeField;

        private bool gTSPayeeFieldSpecified;

        private long extraMonthField;

        private bool extraMonthFieldSpecified;

        private long extraDaysField;

        private bool extraDaysFieldSpecified;

        private long cashDiscountDateOffsetField;

        private bool cashDiscountDateOffsetFieldSpecified;

        private string startFromField;

        private string nTSApprovedField;

        private long eTaxWebSiteField;

        private bool eTaxWebSiteFieldSpecified;

        private string eTaxNumberField;

        private string nTSApprovedNumberField;

        private string eDocGenerationTypeField;

        private long eDocSeriesField;

        private bool eDocSeriesFieldSpecified;

        private long eDocExportFormatField;

        private bool eDocExportFormatFieldSpecified;

        private string eDocStatusField;

        private string eDocErrorCodeField;

        private string eDocErrorMessageField;

        private string downPaymentStatusField;

        private long groupSeriesField;

        private bool groupSeriesFieldSpecified;

        private long groupNumberField;

        private bool groupNumberFieldSpecified;

        private string groupHandWrittenField;

        private string reopenOriginalDocumentField;

        private string reopenManuallyClosedOrCanceledDocumentField;

        private string createOnlineQuotationField;

        private string pOSEquipmentNumberField;

        private string pOSManufacturerSerialNumberField;

        private long pOSCashierNumberField;

        private bool pOSCashierNumberFieldSpecified;

        private string applyCurrentVATRatesForDownPaymentsToDrawField;

        private long closingOptionField;

        private bool closingOptionFieldSpecified;

        private string specifiedClosingDateField;

        private string openForLandedCostsField;

        private string relevantToGTSField;

        private long annualInvoiceDeclarationReferenceField;

        private bool annualInvoiceDeclarationReferenceFieldSpecified;

        private string supplierField;

        private long releaserField;

        private bool releaserFieldSpecified;

        private long receiverField;

        private bool receiverFieldSpecified;

        private long blanketAgreementNumberField;

        private bool blanketAgreementNumberFieldSpecified;

        private string isAlterationField;

        private string assetValueDateField;

        private string documentDeliveryField;

        private string authorizationCodeField;

        private string startDeliveryDateField;

        private long startDeliveryTimeField;

        private bool startDeliveryTimeFieldSpecified;

        private string endDeliveryDateField;

        private long endDeliveryTimeField;

        private bool endDeliveryTimeFieldSpecified;

        private string vehiclePlateField;

        private string aTDocumentTypeField;

        private string elecCommStatusField;

        private string reuseDocumentNumField;

        private string reuseNotaFiscalNumField;

        private string printSEPADirectField;

        private string fiscalDocNumField;

        private long u_B1SYS_ConnTypeCodeField;

        private bool u_B1SYS_ConnTypeCodeFieldSpecified;

        private long u_B1SYS_TensGrpCodeField;

        private bool u_B1SYS_TensGrpCodeFieldSpecified;

        private long u_B1SYS_EnergConsClsField;

        private bool u_B1SYS_EnergConsClsFieldSpecified;

        private string u_B1SYS_ParticipantCField;

        private long u_B1SYS_SubscrCodeField;

        private bool u_B1SYS_SubscrCodeFieldSpecified;

        private string u_B1SYS_MeterCodeField;

        private long u_B1SYS_RevIndC510Field;

        private bool u_B1SYS_RevIndC510FieldSpecified;

        private long u_B1SYS_RevIndD600Field;

        private bool u_B1SYS_RevIndD600FieldSpecified;

        private long u_B1SYS_RevIndD510Field;

        private bool u_B1SYS_RevIndD510FieldSpecified;

        private string u_ChaveAcessoField;

        private string u_Reg_ularizaField;

        private string u_DSORIGEMField;

        private string u_DSDEBField;

        private string u_ContratoField;

        private string u_SafraField;

        /// <remarks/>
        public long DocEntry
        {
            get
            {
                return this.docEntryField;
            }
            set
            {
                this.docEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocEntrySpecified
        {
            get
            {
                return this.docEntryFieldSpecified;
            }
            set
            {
                this.docEntryFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long DocNum
        {
            get
            {
                return this.docNumField;
            }
            set
            {
                this.docNumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocNumSpecified
        {
            get
            {
                return this.docNumFieldSpecified;
            }
            set
            {
                this.docNumFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string DocType
        {
            get
            {
                return this.docTypeField;
            }
            set
            {
                this.docTypeField = value;
            }
        }

        /// <remarks/>
        public string HandWritten
        {
            get
            {
                return this.handWrittenField;
            }
            set
            {
                this.handWrittenField = value;
            }
        }

        /// <remarks/>
        public string Printed
        {
            get
            {
                return this.printedField;
            }
            set
            {
                this.printedField = value;
            }
        }

        /// <remarks/>
        public DateTime DocDate
        {
            get
            {
                return this.docDateField;
            }
            set
            {
                this.docDateField = value;
            }
        }

        /// <remarks/>
        public DateTime DocDueDate
        {
            get
            {
                return this.docDueDateField;
            }
            set
            {
                this.docDueDateField = value;
            }
        }

        /// <remarks/>
        public string CardCode
        {
            get
            {
                return this.cardCodeField;
            }
            set
            {
                this.cardCodeField = value;
            }
        }

        /// <remarks/>
        public string CardName
        {
            get
            {
                return this.cardNameField;
            }
            set
            {
                this.cardNameField = value;
            }
        }

        /// <remarks/>
        public string Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        public string NumAtCard
        {
            get
            {
                return this.numAtCardField;
            }
            set
            {
                this.numAtCardField = value;
            }
        }

        /// <remarks/>
        public double DocTotal
        {
            get
            {
                return this.docTotalField;
            }
            set
            {
                this.docTotalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocTotalSpecified
        {
            get
            {
                return this.docTotalFieldSpecified;
            }
            set
            {
                this.docTotalFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long AttachmentEntry
        {
            get
            {
                return this.attachmentEntryField;
            }
            set
            {
                this.attachmentEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AttachmentEntrySpecified
        {
            get
            {
                return this.attachmentEntryFieldSpecified;
            }
            set
            {
                this.attachmentEntryFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string DocCurrency
        {
            get
            {
                return this.docCurrencyField;
            }
            set
            {
                this.docCurrencyField = value;
            }
        }

        /// <remarks/>
        public double DocRate
        {
            get
            {
                return this.docRateField;
            }
            set
            {
                this.docRateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocRateSpecified
        {
            get
            {
                return this.docRateFieldSpecified;
            }
            set
            {
                this.docRateFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Reference1
        {
            get
            {
                return this.reference1Field;
            }
            set
            {
                this.reference1Field = value;
            }
        }

        /// <remarks/>
        public string Reference2
        {
            get
            {
                return this.reference2Field;
            }
            set
            {
                this.reference2Field = value;
            }
        }

        /// <remarks/>
        public string Comments
        {
            get
            {
                return this.commentsField;
            }
            set
            {
                this.commentsField = value;
            }
        }

        /// <remarks/>
        public string JournalMemo
        {
            get
            {
                return this.journalMemoField;
            }
            set
            {
                this.journalMemoField = value;
            }
        }

        /// <remarks/>
        public long PaymentGroupCode
        {
            get
            {
                return this.paymentGroupCodeField;
            }
            set
            {
                this.paymentGroupCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PaymentGroupCodeSpecified
        {
            get
            {
                return this.paymentGroupCodeFieldSpecified;
            }
            set
            {
                this.paymentGroupCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long DocTime
        {
            get
            {
                return this.docTimeField;
            }
            set
            {
                this.docTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocTimeSpecified
        {
            get
            {
                return this.docTimeFieldSpecified;
            }
            set
            {
                this.docTimeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long SalesPersonCode
        {
            get
            {
                return this.salesPersonCodeField;
            }
            set
            {
                this.salesPersonCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SalesPersonCodeSpecified
        {
            get
            {
                return this.salesPersonCodeFieldSpecified;
            }
            set
            {
                this.salesPersonCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long TransportationCode
        {
            get
            {
                return this.transportationCodeField;
            }
            set
            {
                this.transportationCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TransportationCodeSpecified
        {
            get
            {
                return this.transportationCodeFieldSpecified;
            }
            set
            {
                this.transportationCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Confirmed
        {
            get
            {
                return this.confirmedField;
            }
            set
            {
                this.confirmedField = value;
            }
        }

        /// <remarks/>
        public long ImportFileNum
        {
            get
            {
                return this.importFileNumField;
            }
            set
            {
                this.importFileNumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ImportFileNumSpecified
        {
            get
            {
                return this.importFileNumFieldSpecified;
            }
            set
            {
                this.importFileNumFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string SummeryType
        {
            get
            {
                return this.summeryTypeField;
            }
            set
            {
                this.summeryTypeField = value;
            }
        }

        /// <remarks/>
        public long ContactPersonCode
        {
            get
            {
                return this.contactPersonCodeField;
            }
            set
            {
                this.contactPersonCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ContactPersonCodeSpecified
        {
            get
            {
                return this.contactPersonCodeFieldSpecified;
            }
            set
            {
                this.contactPersonCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ShowSCN
        {
            get
            {
                return this.showSCNField;
            }
            set
            {
                this.showSCNField = value;
            }
        }

        /// <remarks/>
        public long Series
        {
            get
            {
                return this.seriesField;
            }
            set
            {
                this.seriesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SeriesSpecified
        {
            get
            {
                return this.seriesFieldSpecified;
            }
            set
            {
                this.seriesFieldSpecified = value;
            }
        }

        /// <remarks/>
        public DateTime TaxDate
        {
            get
            {
                return this.taxDateField;
            }
            set
            {
                this.taxDateField = value;
            }
        }

        /// <remarks/>
        public string PartialSupply
        {
            get
            {
                return this.partialSupplyField;
            }
            set
            {
                this.partialSupplyField = value;
            }
        }

        /// <remarks/>
        public string DocObjectCode
        {
            get
            {
                return this.docObjectCodeField;
            }
            set
            {
                this.docObjectCodeField = value;
            }
        }

        /// <remarks/>
        public string ShipToCode
        {
            get
            {
                return this.shipToCodeField;
            }
            set
            {
                this.shipToCodeField = value;
            }
        }

        /// <remarks/>
        public string Indicator
        {
            get
            {
                return this.indicatorField;
            }
            set
            {
                this.indicatorField = value;
            }
        }

        /// <remarks/>
        public string FederalTaxID
        {
            get
            {
                return this.federalTaxIDField;
            }
            set
            {
                this.federalTaxIDField = value;
            }
        }

        /// <remarks/>
        public double DiscountPercent
        {
            get
            {
                return this.discountPercentField;
            }
            set
            {
                this.discountPercentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DiscountPercentSpecified
        {
            get
            {
                return this.discountPercentFieldSpecified;
            }
            set
            {
                this.discountPercentFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string PaymentReference
        {
            get
            {
                return this.paymentReferenceField;
            }
            set
            {
                this.paymentReferenceField = value;
            }
        }

        /// <remarks/>
        public double DocTotalFc
        {
            get
            {
                return this.docTotalFcField;
            }
            set
            {
                this.docTotalFcField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocTotalFcSpecified
        {
            get
            {
                return this.docTotalFcFieldSpecified;
            }
            set
            {
                this.docTotalFcFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Form1099
        {
            get
            {
                return this.form1099Field;
            }
            set
            {
                this.form1099Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Form1099Specified
        {
            get
            {
                return this.form1099FieldSpecified;
            }
            set
            {
                this.form1099FieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Box1099
        {
            get
            {
                return this.box1099Field;
            }
            set
            {
                this.box1099Field = value;
            }
        }

        /// <remarks/>
        public string RevisionPo
        {
            get
            {
                return this.revisionPoField;
            }
            set
            {
                this.revisionPoField = value;
            }
        }

        /// <remarks/>
        public string RequriedDate
        {
            get
            {
                return this.requriedDateField;
            }
            set
            {
                this.requriedDateField = value;
            }
        }

        /// <remarks/>
        public string CancelDate
        {
            get
            {
                return this.cancelDateField;
            }
            set
            {
                this.cancelDateField = value;
            }
        }

        /// <remarks/>
        public string BlockDunning
        {
            get
            {
                return this.blockDunningField;
            }
            set
            {
                this.blockDunningField = value;
            }
        }

        /// <remarks/>
        public string Pick
        {
            get
            {
                return this.pickField;
            }
            set
            {
                this.pickField = value;
            }
        }

        /// <remarks/>
        public string PaymentMethod
        {
            get
            {
                return this.paymentMethodField;
            }
            set
            {
                this.paymentMethodField = value;
            }
        }

        /// <remarks/>
        public string PaymentBlock
        {
            get
            {
                return this.paymentBlockField;
            }
            set
            {
                this.paymentBlockField = value;
            }
        }

        /// <remarks/>
        public long PaymentBlockEntry
        {
            get
            {
                return this.paymentBlockEntryField;
            }
            set
            {
                this.paymentBlockEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PaymentBlockEntrySpecified
        {
            get
            {
                return this.paymentBlockEntryFieldSpecified;
            }
            set
            {
                this.paymentBlockEntryFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string CentralBankIndicator
        {
            get
            {
                return this.centralBankIndicatorField;
            }
            set
            {
                this.centralBankIndicatorField = value;
            }
        }

        /// <remarks/>
        public string MaximumCashDiscount
        {
            get
            {
                return this.maximumCashDiscountField;
            }
            set
            {
                this.maximumCashDiscountField = value;
            }
        }

        /// <remarks/>
        public string Project
        {
            get
            {
                return this.projectField;
            }
            set
            {
                this.projectField = value;
            }
        }

        /// <remarks/>
        public string ExemptionValidityDateFrom
        {
            get
            {
                return this.exemptionValidityDateFromField;
            }
            set
            {
                this.exemptionValidityDateFromField = value;
            }
        }

        /// <remarks/>
        public string ExemptionValidityDateTo
        {
            get
            {
                return this.exemptionValidityDateToField;
            }
            set
            {
                this.exemptionValidityDateToField = value;
            }
        }

        /// <remarks/>
        public string WareHouseUpdateType
        {
            get
            {
                return this.wareHouseUpdateTypeField;
            }
            set
            {
                this.wareHouseUpdateTypeField = value;
            }
        }

        /// <remarks/>
        public string Rounding
        {
            get
            {
                return this.roundingField;
            }
            set
            {
                this.roundingField = value;
            }
        }

        /// <remarks/>
        public string ExternalCorrectedDocNum
        {
            get
            {
                return this.externalCorrectedDocNumField;
            }
            set
            {
                this.externalCorrectedDocNumField = value;
            }
        }

        /// <remarks/>
        public long InternalCorrectedDocNum
        {
            get
            {
                return this.internalCorrectedDocNumField;
            }
            set
            {
                this.internalCorrectedDocNumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool InternalCorrectedDocNumSpecified
        {
            get
            {
                return this.internalCorrectedDocNumFieldSpecified;
            }
            set
            {
                this.internalCorrectedDocNumFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string DeferredTax
        {
            get
            {
                return this.deferredTaxField;
            }
            set
            {
                this.deferredTaxField = value;
            }
        }

        /// <remarks/>
        public string TaxExemptionLetterNum
        {
            get
            {
                return this.taxExemptionLetterNumField;
            }
            set
            {
                this.taxExemptionLetterNumField = value;
            }
        }

        /// <remarks/>
        public string AgentCode
        {
            get
            {
                return this.agentCodeField;
            }
            set
            {
                this.agentCodeField = value;
            }
        }

        /// <remarks/>
        public long NumberOfInstallments
        {
            get
            {
                return this.numberOfInstallmentsField;
            }
            set
            {
                this.numberOfInstallmentsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NumberOfInstallmentsSpecified
        {
            get
            {
                return this.numberOfInstallmentsFieldSpecified;
            }
            set
            {
                this.numberOfInstallmentsFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ApplyTaxOnFirstInstallment
        {
            get
            {
                return this.applyTaxOnFirstInstallmentField;
            }
            set
            {
                this.applyTaxOnFirstInstallmentField = value;
            }
        }

        /// <remarks/>
        public string VatDate
        {
            get
            {
                return this.vatDateField;
            }
            set
            {
                this.vatDateField = value;
            }
        }

        /// <remarks/>
        public long DocumentsOwner
        {
            get
            {
                return this.documentsOwnerField;
            }
            set
            {
                this.documentsOwnerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocumentsOwnerSpecified
        {
            get
            {
                return this.documentsOwnerFieldSpecified;
            }
            set
            {
                this.documentsOwnerFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string FolioPrefixString
        {
            get
            {
                return this.folioPrefixStringField;
            }
            set
            {
                this.folioPrefixStringField = value;
            }
        }

        /// <remarks/>
        public long FolioNumber
        {
            get
            {
                return this.folioNumberField;
            }
            set
            {
                this.folioNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FolioNumberSpecified
        {
            get
            {
                return this.folioNumberFieldSpecified;
            }
            set
            {
                this.folioNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string DocumentSubType
        {
            get
            {
                return this.documentSubTypeField;
            }
            set
            {
                this.documentSubTypeField = value;
            }
        }

        /// <remarks/>
        public string BPChannelCode
        {
            get
            {
                return this.bPChannelCodeField;
            }
            set
            {
                this.bPChannelCodeField = value;
            }
        }

        /// <remarks/>
        public long BPChannelContact
        {
            get
            {
                return this.bPChannelContactField;
            }
            set
            {
                this.bPChannelContactField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BPChannelContactSpecified
        {
            get
            {
                return this.bPChannelContactFieldSpecified;
            }
            set
            {
                this.bPChannelContactFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Address2
        {
            get
            {
                return this.address2Field;
            }
            set
            {
                this.address2Field = value;
            }
        }

        /// <remarks/>
        public string PayToCode
        {
            get
            {
                return this.payToCodeField;
            }
            set
            {
                this.payToCodeField = value;
            }
        }

        /// <remarks/>
        public string ManualNumber
        {
            get
            {
                return this.manualNumberField;
            }
            set
            {
                this.manualNumberField = value;
            }
        }

        /// <remarks/>
        public string UseShpdGoodsAct
        {
            get
            {
                return this.useShpdGoodsActField;
            }
            set
            {
                this.useShpdGoodsActField = value;
            }
        }

        /// <remarks/>
        public string IsPayToBank
        {
            get
            {
                return this.isPayToBankField;
            }
            set
            {
                this.isPayToBankField = value;
            }
        }

        /// <remarks/>
        public string PayToBankCountry
        {
            get
            {
                return this.payToBankCountryField;
            }
            set
            {
                this.payToBankCountryField = value;
            }
        }

        /// <remarks/>
        public string PayToBankCode
        {
            get
            {
                return this.payToBankCodeField;
            }
            set
            {
                this.payToBankCodeField = value;
            }
        }

        /// <remarks/>
        public string PayToBankAccountNo
        {
            get
            {
                return this.payToBankAccountNoField;
            }
            set
            {
                this.payToBankAccountNoField = value;
            }
        }

        /// <remarks/>
        public string PayToBankBranch
        {
            get
            {
                return this.payToBankBranchField;
            }
            set
            {
                this.payToBankBranchField = value;
            }
        }

        /// <remarks/>
        public long BPL_IDAssignedToInvoice
        {
            get
            {
                return this.bPL_IDAssignedToInvoiceField;
            }
            set
            {
                this.bPL_IDAssignedToInvoiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BPL_IDAssignedToInvoiceSpecified
        {
            get
            {
                return this.bPL_IDAssignedToInvoiceFieldSpecified;
            }
            set
            {
                this.bPL_IDAssignedToInvoiceFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double DownPayment
        {
            get
            {
                return this.downPaymentField;
            }
            set
            {
                this.downPaymentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DownPaymentSpecified
        {
            get
            {
                return this.downPaymentFieldSpecified;
            }
            set
            {
                this.downPaymentFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ReserveInvoice
        {
            get
            {
                return this.reserveInvoiceField;
            }
            set
            {
                this.reserveInvoiceField = value;
            }
        }

        /// <remarks/>
        public long LanguageCode
        {
            get
            {
                return this.languageCodeField;
            }
            set
            {
                this.languageCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LanguageCodeSpecified
        {
            get
            {
                return this.languageCodeFieldSpecified;
            }
            set
            {
                this.languageCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string TrackingNumber
        {
            get
            {
                return this.trackingNumberField;
            }
            set
            {
                this.trackingNumberField = value;
            }
        }

        /// <remarks/>
        public string PickRemark
        {
            get
            {
                return this.pickRemarkField;
            }
            set
            {
                this.pickRemarkField = value;
            }
        }

        /// <remarks/>
        public string ClosingDate
        {
            get
            {
                return this.closingDateField;
            }
            set
            {
                this.closingDateField = value;
            }
        }

        /// <remarks/>
        public long SequenceCode
        {
            get
            {
                return this.sequenceCodeField;
            }
            set
            {
                this.sequenceCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SequenceCodeSpecified
        {
            get
            {
                return this.sequenceCodeFieldSpecified;
            }
            set
            {
                this.sequenceCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long SequenceSerial
        {
            get
            {
                return this.sequenceSerialField;
            }
            set
            {
                this.sequenceSerialField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SequenceSerialSpecified
        {
            get
            {
                return this.sequenceSerialFieldSpecified;
            }
            set
            {
                this.sequenceSerialFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string SeriesString
        {
            get
            {
                return this.seriesStringField;
            }
            set
            {
                this.seriesStringField = value;
            }
        }

        /// <remarks/>
        public string SubSeriesString
        {
            get
            {
                return this.subSeriesStringField;
            }
            set
            {
                this.subSeriesStringField = value;
            }
        }

        /// <remarks/>
        public string SequenceModel
        {
            get
            {
                return this.sequenceModelField;
            }
            set
            {
                this.sequenceModelField = value;
            }
        }

        /// <remarks/>
        public string UseCorrectionVATGroup
        {
            get
            {
                return this.useCorrectionVATGroupField;
            }
            set
            {
                this.useCorrectionVATGroupField = value;
            }
        }

        /// <remarks/>
        public double DownPaymentAmount
        {
            get
            {
                return this.downPaymentAmountField;
            }
            set
            {
                this.downPaymentAmountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DownPaymentAmountSpecified
        {
            get
            {
                return this.downPaymentAmountFieldSpecified;
            }
            set
            {
                this.downPaymentAmountFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double DownPaymentPercentage
        {
            get
            {
                return this.downPaymentPercentageField;
            }
            set
            {
                this.downPaymentPercentageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DownPaymentPercentageSpecified
        {
            get
            {
                return this.downPaymentPercentageFieldSpecified;
            }
            set
            {
                this.downPaymentPercentageFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string DownPaymentType
        {
            get
            {
                return this.downPaymentTypeField;
            }
            set
            {
                this.downPaymentTypeField = value;
            }
        }

        /// <remarks/>
        public double DownPaymentAmountSC
        {
            get
            {
                return this.downPaymentAmountSCField;
            }
            set
            {
                this.downPaymentAmountSCField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DownPaymentAmountSCSpecified
        {
            get
            {
                return this.downPaymentAmountSCFieldSpecified;
            }
            set
            {
                this.downPaymentAmountSCFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double DownPaymentAmountFC
        {
            get
            {
                return this.downPaymentAmountFCField;
            }
            set
            {
                this.downPaymentAmountFCField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DownPaymentAmountFCSpecified
        {
            get
            {
                return this.downPaymentAmountFCFieldSpecified;
            }
            set
            {
                this.downPaymentAmountFCFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double VatPercent
        {
            get
            {
                return this.vatPercentField;
            }
            set
            {
                this.vatPercentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VatPercentSpecified
        {
            get
            {
                return this.vatPercentFieldSpecified;
            }
            set
            {
                this.vatPercentFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double ServiceGrossProfitPercent
        {
            get
            {
                return this.serviceGrossProfitPercentField;
            }
            set
            {
                this.serviceGrossProfitPercentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ServiceGrossProfitPercentSpecified
        {
            get
            {
                return this.serviceGrossProfitPercentFieldSpecified;
            }
            set
            {
                this.serviceGrossProfitPercentFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string OpeningRemarks
        {
            get
            {
                return this.openingRemarksField;
            }
            set
            {
                this.openingRemarksField = value;
            }
        }

        /// <remarks/>
        public string ClosingRemarks
        {
            get
            {
                return this.closingRemarksField;
            }
            set
            {
                this.closingRemarksField = value;
            }
        }

        /// <remarks/>
        public double RoundingDiffAmount
        {
            get
            {
                return this.roundingDiffAmountField;
            }
            set
            {
                this.roundingDiffAmountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RoundingDiffAmountSpecified
        {
            get
            {
                return this.roundingDiffAmountFieldSpecified;
            }
            set
            {
                this.roundingDiffAmountFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ControlAccount
        {
            get
            {
                return this.controlAccountField;
            }
            set
            {
                this.controlAccountField = value;
            }
        }

        /// <remarks/>
        public string InsuranceOperation347
        {
            get
            {
                return this.insuranceOperation347Field;
            }
            set
            {
                this.insuranceOperation347Field = value;
            }
        }

        /// <remarks/>
        public string ArchiveNonremovableSalesQuotation
        {
            get
            {
                return this.archiveNonremovableSalesQuotationField;
            }
            set
            {
                this.archiveNonremovableSalesQuotationField = value;
            }
        }

        /// <remarks/>
        public long GTSChecker
        {
            get
            {
                return this.gTSCheckerField;
            }
            set
            {
                this.gTSCheckerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GTSCheckerSpecified
        {
            get
            {
                return this.gTSCheckerFieldSpecified;
            }
            set
            {
                this.gTSCheckerFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long GTSPayee
        {
            get
            {
                return this.gTSPayeeField;
            }
            set
            {
                this.gTSPayeeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GTSPayeeSpecified
        {
            get
            {
                return this.gTSPayeeFieldSpecified;
            }
            set
            {
                this.gTSPayeeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long ExtraMonth
        {
            get
            {
                return this.extraMonthField;
            }
            set
            {
                this.extraMonthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ExtraMonthSpecified
        {
            get
            {
                return this.extraMonthFieldSpecified;
            }
            set
            {
                this.extraMonthFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long ExtraDays
        {
            get
            {
                return this.extraDaysField;
            }
            set
            {
                this.extraDaysField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ExtraDaysSpecified
        {
            get
            {
                return this.extraDaysFieldSpecified;
            }
            set
            {
                this.extraDaysFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long CashDiscountDateOffset
        {
            get
            {
                return this.cashDiscountDateOffsetField;
            }
            set
            {
                this.cashDiscountDateOffsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CashDiscountDateOffsetSpecified
        {
            get
            {
                return this.cashDiscountDateOffsetFieldSpecified;
            }
            set
            {
                this.cashDiscountDateOffsetFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string StartFrom
        {
            get
            {
                return this.startFromField;
            }
            set
            {
                this.startFromField = value;
            }
        }

        /// <remarks/>
        public string NTSApproved
        {
            get
            {
                return this.nTSApprovedField;
            }
            set
            {
                this.nTSApprovedField = value;
            }
        }

        /// <remarks/>
        public long ETaxWebSite
        {
            get
            {
                return this.eTaxWebSiteField;
            }
            set
            {
                this.eTaxWebSiteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ETaxWebSiteSpecified
        {
            get
            {
                return this.eTaxWebSiteFieldSpecified;
            }
            set
            {
                this.eTaxWebSiteFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ETaxNumber
        {
            get
            {
                return this.eTaxNumberField;
            }
            set
            {
                this.eTaxNumberField = value;
            }
        }

        /// <remarks/>
        public string NTSApprovedNumber
        {
            get
            {
                return this.nTSApprovedNumberField;
            }
            set
            {
                this.nTSApprovedNumberField = value;
            }
        }

        /// <remarks/>
        public string EDocGenerationType
        {
            get
            {
                return this.eDocGenerationTypeField;
            }
            set
            {
                this.eDocGenerationTypeField = value;
            }
        }

        /// <remarks/>
        public long EDocSeries
        {
            get
            {
                return this.eDocSeriesField;
            }
            set
            {
                this.eDocSeriesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EDocSeriesSpecified
        {
            get
            {
                return this.eDocSeriesFieldSpecified;
            }
            set
            {
                this.eDocSeriesFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long EDocExportFormat
        {
            get
            {
                return this.eDocExportFormatField;
            }
            set
            {
                this.eDocExportFormatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EDocExportFormatSpecified
        {
            get
            {
                return this.eDocExportFormatFieldSpecified;
            }
            set
            {
                this.eDocExportFormatFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string EDocStatus
        {
            get
            {
                return this.eDocStatusField;
            }
            set
            {
                this.eDocStatusField = value;
            }
        }

        /// <remarks/>
        public string EDocErrorCode
        {
            get
            {
                return this.eDocErrorCodeField;
            }
            set
            {
                this.eDocErrorCodeField = value;
            }
        }

        /// <remarks/>
        public string EDocErrorMessage
        {
            get
            {
                return this.eDocErrorMessageField;
            }
            set
            {
                this.eDocErrorMessageField = value;
            }
        }

        /// <remarks/>
        public string DownPaymentStatus
        {
            get
            {
                return this.downPaymentStatusField;
            }
            set
            {
                this.downPaymentStatusField = value;
            }
        }

        /// <remarks/>
        public long GroupSeries
        {
            get
            {
                return this.groupSeriesField;
            }
            set
            {
                this.groupSeriesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GroupSeriesSpecified
        {
            get
            {
                return this.groupSeriesFieldSpecified;
            }
            set
            {
                this.groupSeriesFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long GroupNumber
        {
            get
            {
                return this.groupNumberField;
            }
            set
            {
                this.groupNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GroupNumberSpecified
        {
            get
            {
                return this.groupNumberFieldSpecified;
            }
            set
            {
                this.groupNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string GroupHandWritten
        {
            get
            {
                return this.groupHandWrittenField;
            }
            set
            {
                this.groupHandWrittenField = value;
            }
        }

        /// <remarks/>
        public string ReopenOriginalDocument
        {
            get
            {
                return this.reopenOriginalDocumentField;
            }
            set
            {
                this.reopenOriginalDocumentField = value;
            }
        }

        /// <remarks/>
        public string ReopenManuallyClosedOrCanceledDocument
        {
            get
            {
                return this.reopenManuallyClosedOrCanceledDocumentField;
            }
            set
            {
                this.reopenManuallyClosedOrCanceledDocumentField = value;
            }
        }

        /// <remarks/>
        public string CreateOnlineQuotation
        {
            get
            {
                return this.createOnlineQuotationField;
            }
            set
            {
                this.createOnlineQuotationField = value;
            }
        }

        /// <remarks/>
        public string POSEquipmentNumber
        {
            get
            {
                return this.pOSEquipmentNumberField;
            }
            set
            {
                this.pOSEquipmentNumberField = value;
            }
        }

        /// <remarks/>
        public string POSManufacturerSerialNumber
        {
            get
            {
                return this.pOSManufacturerSerialNumberField;
            }
            set
            {
                this.pOSManufacturerSerialNumberField = value;
            }
        }

        /// <remarks/>
        public long POSCashierNumber
        {
            get
            {
                return this.pOSCashierNumberField;
            }
            set
            {
                this.pOSCashierNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool POSCashierNumberSpecified
        {
            get
            {
                return this.pOSCashierNumberFieldSpecified;
            }
            set
            {
                this.pOSCashierNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ApplyCurrentVATRatesForDownPaymentsToDraw
        {
            get
            {
                return this.applyCurrentVATRatesForDownPaymentsToDrawField;
            }
            set
            {
                this.applyCurrentVATRatesForDownPaymentsToDrawField = value;
            }
        }

        /// <remarks/>
        public long ClosingOption
        {
            get
            {
                return this.closingOptionField;
            }
            set
            {
                this.closingOptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ClosingOptionSpecified
        {
            get
            {
                return this.closingOptionFieldSpecified;
            }
            set
            {
                this.closingOptionFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string SpecifiedClosingDate
        {
            get
            {
                return this.specifiedClosingDateField;
            }
            set
            {
                this.specifiedClosingDateField = value;
            }
        }

        /// <remarks/>
        public string OpenForLandedCosts
        {
            get
            {
                return this.openForLandedCostsField;
            }
            set
            {
                this.openForLandedCostsField = value;
            }
        }

        /// <remarks/>
        public string RelevantToGTS
        {
            get
            {
                return this.relevantToGTSField;
            }
            set
            {
                this.relevantToGTSField = value;
            }
        }

        /// <remarks/>
        public long AnnualInvoiceDeclarationReference
        {
            get
            {
                return this.annualInvoiceDeclarationReferenceField;
            }
            set
            {
                this.annualInvoiceDeclarationReferenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AnnualInvoiceDeclarationReferenceSpecified
        {
            get
            {
                return this.annualInvoiceDeclarationReferenceFieldSpecified;
            }
            set
            {
                this.annualInvoiceDeclarationReferenceFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Supplier
        {
            get
            {
                return this.supplierField;
            }
            set
            {
                this.supplierField = value;
            }
        }

        /// <remarks/>
        public long Releaser
        {
            get
            {
                return this.releaserField;
            }
            set
            {
                this.releaserField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ReleaserSpecified
        {
            get
            {
                return this.releaserFieldSpecified;
            }
            set
            {
                this.releaserFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Receiver
        {
            get
            {
                return this.receiverField;
            }
            set
            {
                this.receiverField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ReceiverSpecified
        {
            get
            {
                return this.receiverFieldSpecified;
            }
            set
            {
                this.receiverFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long BlanketAgreementNumber
        {
            get
            {
                return this.blanketAgreementNumberField;
            }
            set
            {
                this.blanketAgreementNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BlanketAgreementNumberSpecified
        {
            get
            {
                return this.blanketAgreementNumberFieldSpecified;
            }
            set
            {
                this.blanketAgreementNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string IsAlteration
        {
            get
            {
                return this.isAlterationField;
            }
            set
            {
                this.isAlterationField = value;
            }
        }

        /// <remarks/>
        public string AssetValueDate
        {
            get
            {
                return this.assetValueDateField;
            }
            set
            {
                this.assetValueDateField = value;
            }
        }

        /// <remarks/>
        public string DocumentDelivery
        {
            get
            {
                return this.documentDeliveryField;
            }
            set
            {
                this.documentDeliveryField = value;
            }
        }

        /// <remarks/>
        public string AuthorizationCode
        {
            get
            {
                return this.authorizationCodeField;
            }
            set
            {
                this.authorizationCodeField = value;
            }
        }

        /// <remarks/>
        public string StartDeliveryDate
        {
            get
            {
                return this.startDeliveryDateField;
            }
            set
            {
                this.startDeliveryDateField = value;
            }
        }

        /// <remarks/>
        public long StartDeliveryTime
        {
            get
            {
                return this.startDeliveryTimeField;
            }
            set
            {
                this.startDeliveryTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StartDeliveryTimeSpecified
        {
            get
            {
                return this.startDeliveryTimeFieldSpecified;
            }
            set
            {
                this.startDeliveryTimeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string EndDeliveryDate
        {
            get
            {
                return this.endDeliveryDateField;
            }
            set
            {
                this.endDeliveryDateField = value;
            }
        }

        /// <remarks/>
        public long EndDeliveryTime
        {
            get
            {
                return this.endDeliveryTimeField;
            }
            set
            {
                this.endDeliveryTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EndDeliveryTimeSpecified
        {
            get
            {
                return this.endDeliveryTimeFieldSpecified;
            }
            set
            {
                this.endDeliveryTimeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string VehiclePlate
        {
            get
            {
                return this.vehiclePlateField;
            }
            set
            {
                this.vehiclePlateField = value;
            }
        }

        /// <remarks/>
        public string ATDocumentType
        {
            get
            {
                return this.aTDocumentTypeField;
            }
            set
            {
                this.aTDocumentTypeField = value;
            }
        }

        /// <remarks/>
        public string ElecCommStatus
        {
            get
            {
                return this.elecCommStatusField;
            }
            set
            {
                this.elecCommStatusField = value;
            }
        }

        /// <remarks/>
        public string ReuseDocumentNum
        {
            get
            {
                return this.reuseDocumentNumField;
            }
            set
            {
                this.reuseDocumentNumField = value;
            }
        }

        /// <remarks/>
        public string ReuseNotaFiscalNum
        {
            get
            {
                return this.reuseNotaFiscalNumField;
            }
            set
            {
                this.reuseNotaFiscalNumField = value;
            }
        }

        /// <remarks/>
        public string PrintSEPADirect
        {
            get
            {
                return this.printSEPADirectField;
            }
            set
            {
                this.printSEPADirectField = value;
            }
        }

        /// <remarks/>
        public string FiscalDocNum
        {
            get
            {
                return this.fiscalDocNumField;
            }
            set
            {
                this.fiscalDocNumField = value;
            }
        }

        /// <remarks/>
        public long U_B1SYS_ConnTypeCode
        {
            get
            {
                return this.u_B1SYS_ConnTypeCodeField;
            }
            set
            {
                this.u_B1SYS_ConnTypeCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_B1SYS_ConnTypeCodeSpecified
        {
            get
            {
                return this.u_B1SYS_ConnTypeCodeFieldSpecified;
            }
            set
            {
                this.u_B1SYS_ConnTypeCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long U_B1SYS_TensGrpCode
        {
            get
            {
                return this.u_B1SYS_TensGrpCodeField;
            }
            set
            {
                this.u_B1SYS_TensGrpCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_B1SYS_TensGrpCodeSpecified
        {
            get
            {
                return this.u_B1SYS_TensGrpCodeFieldSpecified;
            }
            set
            {
                this.u_B1SYS_TensGrpCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long U_B1SYS_EnergConsCls
        {
            get
            {
                return this.u_B1SYS_EnergConsClsField;
            }
            set
            {
                this.u_B1SYS_EnergConsClsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_B1SYS_EnergConsClsSpecified
        {
            get
            {
                return this.u_B1SYS_EnergConsClsFieldSpecified;
            }
            set
            {
                this.u_B1SYS_EnergConsClsFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string U_B1SYS_ParticipantC
        {
            get
            {
                return this.u_B1SYS_ParticipantCField;
            }
            set
            {
                this.u_B1SYS_ParticipantCField = value;
            }
        }

        /// <remarks/>
        public long U_B1SYS_SubscrCode
        {
            get
            {
                return this.u_B1SYS_SubscrCodeField;
            }
            set
            {
                this.u_B1SYS_SubscrCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_B1SYS_SubscrCodeSpecified
        {
            get
            {
                return this.u_B1SYS_SubscrCodeFieldSpecified;
            }
            set
            {
                this.u_B1SYS_SubscrCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string U_B1SYS_MeterCode
        {
            get
            {
                return this.u_B1SYS_MeterCodeField;
            }
            set
            {
                this.u_B1SYS_MeterCodeField = value;
            }
        }

        /// <remarks/>
        public long U_B1SYS_RevIndC510
        {
            get
            {
                return this.u_B1SYS_RevIndC510Field;
            }
            set
            {
                this.u_B1SYS_RevIndC510Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_B1SYS_RevIndC510Specified
        {
            get
            {
                return this.u_B1SYS_RevIndC510FieldSpecified;
            }
            set
            {
                this.u_B1SYS_RevIndC510FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long U_B1SYS_RevIndD600
        {
            get
            {
                return this.u_B1SYS_RevIndD600Field;
            }
            set
            {
                this.u_B1SYS_RevIndD600Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_B1SYS_RevIndD600Specified
        {
            get
            {
                return this.u_B1SYS_RevIndD600FieldSpecified;
            }
            set
            {
                this.u_B1SYS_RevIndD600FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long U_B1SYS_RevIndD510
        {
            get
            {
                return this.u_B1SYS_RevIndD510Field;
            }
            set
            {
                this.u_B1SYS_RevIndD510Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_B1SYS_RevIndD510Specified
        {
            get
            {
                return this.u_B1SYS_RevIndD510FieldSpecified;
            }
            set
            {
                this.u_B1SYS_RevIndD510FieldSpecified = value;
            }
        }

        /// <remarks/>
        public string U_ChaveAcesso
        {
            get
            {
                return this.u_ChaveAcessoField;
            }
            set
            {
                this.u_ChaveAcessoField = value;
            }
        }

        /// <remarks/>
        public string U_Reg_ulariza
        {
            get
            {
                return this.u_Reg_ularizaField;
            }
            set
            {
                this.u_Reg_ularizaField = value;
            }
        }

        /// <remarks/>
        public string U_DSORIGEM
        {
            get
            {
                return this.u_DSORIGEMField;
            }
            set
            {
                this.u_DSORIGEMField = value;
            }
        }

        /// <remarks/>
        public string U_DSDEB
        {
            get
            {
                return this.u_DSDEBField;
            }
            set
            {
                this.u_DSDEBField = value;
            }
        }

        /// <remarks/>
        public string U_Contrato
        {
            get
            {
                return this.u_ContratoField;
            }
            set
            {
                this.u_ContratoField = value;
            }
        }

        /// <remarks/>
        public string U_Safra
        {
            get
            {
                return this.u_SafraField;
            }
            set
            {
                this.u_SafraField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DocumentApprovalRequestsField
    {

        private string remarksField;

        /// <remarks/>
        public string Remarks
        {
            get
            {
                return this.remarksField;
            }
            set
            {
                this.remarksField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DocumentLine
    {

        private long lineNumField;

        private bool lineNumFieldSpecified;

        private string itemCodeField;

        private string itemDescriptionField;

        private double quantityField;

        private bool quantityFieldSpecified;

        private string shipDateField;

        private double priceField;

        private bool priceFieldSpecified;

        private double priceAfterVATField;

        private bool priceAfterVATFieldSpecified;

        private string currencyField;

        private double rateField;

        private bool rateFieldSpecified;

        private double discountPercentField;

        private bool discountPercentFieldSpecified;

        private string vendorNumField;

        private string serialNumField;

        private string warehouseCodeField;

        private long salesPersonCodeField;

        private bool salesPersonCodeFieldSpecified;

        private double commisionPercentField;

        private bool commisionPercentFieldSpecified;

        private string treeTypeField;

        private string accountCodeField;

        private string useBaseUnitsField;

        private string supplierCatNumField;

        private string costingCodeField;

        private string projectCodeField;

        private string barCodeField;

        private string vatGroupField;

        private double height1Field;

        private bool height1FieldSpecified;

        private long hight1UnitField;

        private bool hight1UnitFieldSpecified;

        private double height2Field;

        private bool height2FieldSpecified;

        private long height2UnitField;

        private bool height2UnitFieldSpecified;

        private double lengh1Field;

        private bool lengh1FieldSpecified;

        private long lengh1UnitField;

        private bool lengh1UnitFieldSpecified;

        private double lengh2Field;

        private bool lengh2FieldSpecified;

        private long lengh2UnitField;

        private bool lengh2UnitFieldSpecified;

        private double weight1Field;

        private bool weight1FieldSpecified;

        private long weight1UnitField;

        private bool weight1UnitFieldSpecified;

        private double weight2Field;

        private bool weight2FieldSpecified;

        private long weight2UnitField;

        private bool weight2UnitFieldSpecified;

        private double factor1Field;

        private bool factor1FieldSpecified;

        private double factor2Field;

        private bool factor2FieldSpecified;

        private double factor3Field;

        private bool factor3FieldSpecified;

        private double factor4Field;

        private bool factor4FieldSpecified;

        private long baseTypeField;

        private bool baseTypeFieldSpecified;

        private long baseEntryField;

        private bool baseEntryFieldSpecified;

        private long baseLineField;

        private bool baseLineFieldSpecified;

        private double volumeField;

        private bool volumeFieldSpecified;

        private long volumeUnitField;

        private bool volumeUnitFieldSpecified;

        private double width1Field;

        private bool width1FieldSpecified;

        private long width1UnitField;

        private bool width1UnitFieldSpecified;

        private double width2Field;

        private bool width2FieldSpecified;

        private long width2UnitField;

        private bool width2UnitFieldSpecified;

        private string addressField;

        private string taxCodeField;

        private string taxTypeField;

        private string taxLiableField;

        private string backOrderField;

        private string freeTextField;

        private long shippingMethodField;

        private bool shippingMethodFieldSpecified;

        private string correctionInvoiceItemField;

        private double corrInvAmountToStockField;

        private bool corrInvAmountToStockFieldSpecified;

        private double corrInvAmountToDiffAcctField;

        private bool corrInvAmountToDiffAcctFieldSpecified;

        private string wTLiableField;

        private string deferredTaxField;

        private string measureUnitField;

        private double unitsOfMeasurmentField;

        private bool unitsOfMeasurmentFieldSpecified;

        private double lineTotalField;

        private bool lineTotalFieldSpecified;

        private double taxPercentagePerRowField;

        private bool taxPercentagePerRowFieldSpecified;

        private double taxTotalField;

        private bool taxTotalFieldSpecified;

        private string consumerSalesForecastField;

        private double exciseAmountField;

        private bool exciseAmountFieldSpecified;

        private string countryOrgField;

        private string sWWField;

        private string transactionTypeField;

        private string distributeExpenseField;

        private string shipToCodeField;

        private double rowTotalFCField;

        private bool rowTotalFCFieldSpecified;

        private string cFOPCodeField;

        private string cSTCodeField;

        private long usageField;

        private bool usageFieldSpecified;

        private string taxOnlyField;

        private double unitPriceField;

        private bool unitPriceFieldSpecified;

        private string lineStatusField;

        private string lineTypeField;

        private string cOGSCostingCodeField;

        private string cOGSAccountCodeField;

        private string changeAssemlyBoMWarehouseField;

        private double grossBuyPriceField;

        private bool grossBuyPriceFieldSpecified;

        private long grossBaseField;

        private bool grossBaseFieldSpecified;

        private double grossProfitTotalBasePriceField;

        private bool grossProfitTotalBasePriceFieldSpecified;

        private string costingCode2Field;

        private string costingCode3Field;

        private string costingCode4Field;

        private string costingCode5Field;

        private string itemDetailsField;

        private long locationCodeField;

        private bool locationCodeFieldSpecified;

        private string actualDeliveryDateField;

        private string exLineNoField;

        private string requiredDateField;

        private double requiredQuantityField;

        private bool requiredQuantityFieldSpecified;

        private string cOGSCostingCode2Field;

        private string cOGSCostingCode3Field;

        private string cOGSCostingCode4Field;

        private string cOGSCostingCode5Field;

        private string cSTforIPIField;

        private string cSTforPISField;

        private string cSTforCOFINSField;

        private string creditOriginCodeField;

        private string withoutInventoryMovementField;

        private long agreementNoField;

        private bool agreementNoFieldSpecified;

        private long agreementRowNumberField;

        private bool agreementRowNumberFieldSpecified;

        private string shipToDescriptionField;

        private long actualBaseEntryField;

        private bool actualBaseEntryFieldSpecified;

        private long actualBaseLineField;

        private bool actualBaseLineFieldSpecified;

        private long docEntryField;

        private bool docEntryFieldSpecified;

        private double surplusesField;

        private bool surplusesFieldSpecified;

        private double defectAndBreakupField;

        private bool defectAndBreakupFieldSpecified;

        private double shortagesField;

        private bool shortagesFieldSpecified;

        private string considerQuantityField;

        private string partialRetirementField;

        private double retirementQuantityField;

        private bool retirementQuantityFieldSpecified;

        private double retirementAPCField;

        private bool retirementAPCFieldSpecified;

        private long uoMEntryField;

        private bool uoMEntryFieldSpecified;

        private double inventoryQuantityField;

        private bool inventoryQuantityFieldSpecified;

        private long incotermsField;

        private bool incotermsFieldSpecified;

        private long transportModeField;

        private bool transportModeFieldSpecified;

        /// <remarks/>
        public long LineNum
        {
            get
            {
                return this.lineNumField;
            }
            set
            {
                this.lineNumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LineNumSpecified
        {
            get
            {
                return this.lineNumFieldSpecified;
            }
            set
            {
                this.lineNumFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ItemCode
        {
            get
            {
                return this.itemCodeField;
            }
            set
            {
                this.itemCodeField = value;
            }
        }

        /// <remarks/>
        public string ItemDescription
        {
            get
            {
                return this.itemDescriptionField;
            }
            set
            {
                this.itemDescriptionField = value;
            }
        }

        /// <remarks/>
        public double Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool QuantitySpecified
        {
            get
            {
                return this.quantityFieldSpecified;
            }
            set
            {
                this.quantityFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ShipDate
        {
            get
            {
                return this.shipDateField;
            }
            set
            {
                this.shipDateField = value;
            }
        }

        /// <remarks/>
        public double Price
        {
            get
            {
                return this.priceField;
            }
            set
            {
                this.priceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PriceSpecified
        {
            get
            {
                return this.priceFieldSpecified;
            }
            set
            {
                this.priceFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double PriceAfterVAT
        {
            get
            {
                return this.priceAfterVATField;
            }
            set
            {
                this.priceAfterVATField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PriceAfterVATSpecified
        {
            get
            {
                return this.priceAfterVATFieldSpecified;
            }
            set
            {
                this.priceAfterVATFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Currency
        {
            get
            {
                return this.currencyField;
            }
            set
            {
                this.currencyField = value;
            }
        }

        /// <remarks/>
        public double Rate
        {
            get
            {
                return this.rateField;
            }
            set
            {
                this.rateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RateSpecified
        {
            get
            {
                return this.rateFieldSpecified;
            }
            set
            {
                this.rateFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double DiscountPercent
        {
            get
            {
                return this.discountPercentField;
            }
            set
            {
                this.discountPercentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DiscountPercentSpecified
        {
            get
            {
                return this.discountPercentFieldSpecified;
            }
            set
            {
                this.discountPercentFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string VendorNum
        {
            get
            {
                return this.vendorNumField;
            }
            set
            {
                this.vendorNumField = value;
            }
        }

        /// <remarks/>
        public string SerialNum
        {
            get
            {
                return this.serialNumField;
            }
            set
            {
                this.serialNumField = value;
            }
        }

        /// <remarks/>
        public string WarehouseCode
        {
            get
            {
                return this.warehouseCodeField;
            }
            set
            {
                this.warehouseCodeField = value;
            }
        }

        /// <remarks/>
        public long SalesPersonCode
        {
            get
            {
                return this.salesPersonCodeField;
            }
            set
            {
                this.salesPersonCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SalesPersonCodeSpecified
        {
            get
            {
                return this.salesPersonCodeFieldSpecified;
            }
            set
            {
                this.salesPersonCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double CommisionPercent
        {
            get
            {
                return this.commisionPercentField;
            }
            set
            {
                this.commisionPercentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CommisionPercentSpecified
        {
            get
            {
                return this.commisionPercentFieldSpecified;
            }
            set
            {
                this.commisionPercentFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string TreeType
        {
            get
            {
                return this.treeTypeField;
            }
            set
            {
                this.treeTypeField = value;
            }
        }

        /// <remarks/>
        public string AccountCode
        {
            get
            {
                return this.accountCodeField;
            }
            set
            {
                this.accountCodeField = value;
            }
        }

        /// <remarks/>
        public string UseBaseUnits
        {
            get
            {
                return this.useBaseUnitsField;
            }
            set
            {
                this.useBaseUnitsField = value;
            }
        }

        /// <remarks/>
        public string SupplierCatNum
        {
            get
            {
                return this.supplierCatNumField;
            }
            set
            {
                this.supplierCatNumField = value;
            }
        }

        /// <remarks/>
        public string CostingCode
        {
            get
            {
                return this.costingCodeField;
            }
            set
            {
                this.costingCodeField = value;
            }
        }

        /// <remarks/>
        public string ProjectCode
        {
            get
            {
                return this.projectCodeField;
            }
            set
            {
                this.projectCodeField = value;
            }
        }

        /// <remarks/>
        public string BarCode
        {
            get
            {
                return this.barCodeField;
            }
            set
            {
                this.barCodeField = value;
            }
        }

        /// <remarks/>
        public string VatGroup
        {
            get
            {
                return this.vatGroupField;
            }
            set
            {
                this.vatGroupField = value;
            }
        }

        /// <remarks/>
        public double Height1
        {
            get
            {
                return this.height1Field;
            }
            set
            {
                this.height1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Height1Specified
        {
            get
            {
                return this.height1FieldSpecified;
            }
            set
            {
                this.height1FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Hight1Unit
        {
            get
            {
                return this.hight1UnitField;
            }
            set
            {
                this.hight1UnitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Hight1UnitSpecified
        {
            get
            {
                return this.hight1UnitFieldSpecified;
            }
            set
            {
                this.hight1UnitFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Height2
        {
            get
            {
                return this.height2Field;
            }
            set
            {
                this.height2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Height2Specified
        {
            get
            {
                return this.height2FieldSpecified;
            }
            set
            {
                this.height2FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Height2Unit
        {
            get
            {
                return this.height2UnitField;
            }
            set
            {
                this.height2UnitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Height2UnitSpecified
        {
            get
            {
                return this.height2UnitFieldSpecified;
            }
            set
            {
                this.height2UnitFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Lengh1
        {
            get
            {
                return this.lengh1Field;
            }
            set
            {
                this.lengh1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Lengh1Specified
        {
            get
            {
                return this.lengh1FieldSpecified;
            }
            set
            {
                this.lengh1FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Lengh1Unit
        {
            get
            {
                return this.lengh1UnitField;
            }
            set
            {
                this.lengh1UnitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Lengh1UnitSpecified
        {
            get
            {
                return this.lengh1UnitFieldSpecified;
            }
            set
            {
                this.lengh1UnitFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Lengh2
        {
            get
            {
                return this.lengh2Field;
            }
            set
            {
                this.lengh2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Lengh2Specified
        {
            get
            {
                return this.lengh2FieldSpecified;
            }
            set
            {
                this.lengh2FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Lengh2Unit
        {
            get
            {
                return this.lengh2UnitField;
            }
            set
            {
                this.lengh2UnitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Lengh2UnitSpecified
        {
            get
            {
                return this.lengh2UnitFieldSpecified;
            }
            set
            {
                this.lengh2UnitFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Weight1
        {
            get
            {
                return this.weight1Field;
            }
            set
            {
                this.weight1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Weight1Specified
        {
            get
            {
                return this.weight1FieldSpecified;
            }
            set
            {
                this.weight1FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Weight1Unit
        {
            get
            {
                return this.weight1UnitField;
            }
            set
            {
                this.weight1UnitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Weight1UnitSpecified
        {
            get
            {
                return this.weight1UnitFieldSpecified;
            }
            set
            {
                this.weight1UnitFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Weight2
        {
            get
            {
                return this.weight2Field;
            }
            set
            {
                this.weight2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Weight2Specified
        {
            get
            {
                return this.weight2FieldSpecified;
            }
            set
            {
                this.weight2FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Weight2Unit
        {
            get
            {
                return this.weight2UnitField;
            }
            set
            {
                this.weight2UnitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Weight2UnitSpecified
        {
            get
            {
                return this.weight2UnitFieldSpecified;
            }
            set
            {
                this.weight2UnitFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Factor1
        {
            get
            {
                return this.factor1Field;
            }
            set
            {
                this.factor1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Factor1Specified
        {
            get
            {
                return this.factor1FieldSpecified;
            }
            set
            {
                this.factor1FieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Factor2
        {
            get
            {
                return this.factor2Field;
            }
            set
            {
                this.factor2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Factor2Specified
        {
            get
            {
                return this.factor2FieldSpecified;
            }
            set
            {
                this.factor2FieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Factor3
        {
            get
            {
                return this.factor3Field;
            }
            set
            {
                this.factor3Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Factor3Specified
        {
            get
            {
                return this.factor3FieldSpecified;
            }
            set
            {
                this.factor3FieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Factor4
        {
            get
            {
                return this.factor4Field;
            }
            set
            {
                this.factor4Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Factor4Specified
        {
            get
            {
                return this.factor4FieldSpecified;
            }
            set
            {
                this.factor4FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long BaseType
        {
            get
            {
                return this.baseTypeField;
            }
            set
            {
                this.baseTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BaseTypeSpecified
        {
            get
            {
                return this.baseTypeFieldSpecified;
            }
            set
            {
                this.baseTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long BaseEntry
        {
            get
            {
                return this.baseEntryField;
            }
            set
            {
                this.baseEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BaseEntrySpecified
        {
            get
            {
                return this.baseEntryFieldSpecified;
            }
            set
            {
                this.baseEntryFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long BaseLine
        {
            get
            {
                return this.baseLineField;
            }
            set
            {
                this.baseLineField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BaseLineSpecified
        {
            get
            {
                return this.baseLineFieldSpecified;
            }
            set
            {
                this.baseLineFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Volume
        {
            get
            {
                return this.volumeField;
            }
            set
            {
                this.volumeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VolumeSpecified
        {
            get
            {
                return this.volumeFieldSpecified;
            }
            set
            {
                this.volumeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long VolumeUnit
        {
            get
            {
                return this.volumeUnitField;
            }
            set
            {
                this.volumeUnitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VolumeUnitSpecified
        {
            get
            {
                return this.volumeUnitFieldSpecified;
            }
            set
            {
                this.volumeUnitFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Width1
        {
            get
            {
                return this.width1Field;
            }
            set
            {
                this.width1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Width1Specified
        {
            get
            {
                return this.width1FieldSpecified;
            }
            set
            {
                this.width1FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Width1Unit
        {
            get
            {
                return this.width1UnitField;
            }
            set
            {
                this.width1UnitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Width1UnitSpecified
        {
            get
            {
                return this.width1UnitFieldSpecified;
            }
            set
            {
                this.width1UnitFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Width2
        {
            get
            {
                return this.width2Field;
            }
            set
            {
                this.width2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Width2Specified
        {
            get
            {
                return this.width2FieldSpecified;
            }
            set
            {
                this.width2FieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Width2Unit
        {
            get
            {
                return this.width2UnitField;
            }
            set
            {
                this.width2UnitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Width2UnitSpecified
        {
            get
            {
                return this.width2UnitFieldSpecified;
            }
            set
            {
                this.width2UnitFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        public string TaxCode
        {
            get
            {
                return this.taxCodeField;
            }
            set
            {
                this.taxCodeField = value;
            }
        }

        /// <remarks/>
        public string TaxType
        {
            get
            {
                return this.taxTypeField;
            }
            set
            {
                this.taxTypeField = value;
            }
        }

        /// <remarks/>
        public string TaxLiable
        {
            get
            {
                return this.taxLiableField;
            }
            set
            {
                this.taxLiableField = value;
            }
        }

        /// <remarks/>
        public string BackOrder
        {
            get
            {
                return this.backOrderField;
            }
            set
            {
                this.backOrderField = value;
            }
        }

        /// <remarks/>
        public string FreeText
        {
            get
            {
                return this.freeTextField;
            }
            set
            {
                this.freeTextField = value;
            }
        }

        /// <remarks/>
        public long ShippingMethod
        {
            get
            {
                return this.shippingMethodField;
            }
            set
            {
                this.shippingMethodField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShippingMethodSpecified
        {
            get
            {
                return this.shippingMethodFieldSpecified;
            }
            set
            {
                this.shippingMethodFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string CorrectionInvoiceItem
        {
            get
            {
                return this.correctionInvoiceItemField;
            }
            set
            {
                this.correctionInvoiceItemField = value;
            }
        }

        /// <remarks/>
        public double CorrInvAmountToStock
        {
            get
            {
                return this.corrInvAmountToStockField;
            }
            set
            {
                this.corrInvAmountToStockField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CorrInvAmountToStockSpecified
        {
            get
            {
                return this.corrInvAmountToStockFieldSpecified;
            }
            set
            {
                this.corrInvAmountToStockFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double CorrInvAmountToDiffAcct
        {
            get
            {
                return this.corrInvAmountToDiffAcctField;
            }
            set
            {
                this.corrInvAmountToDiffAcctField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CorrInvAmountToDiffAcctSpecified
        {
            get
            {
                return this.corrInvAmountToDiffAcctFieldSpecified;
            }
            set
            {
                this.corrInvAmountToDiffAcctFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string WTLiable
        {
            get
            {
                return this.wTLiableField;
            }
            set
            {
                this.wTLiableField = value;
            }
        }

        /// <remarks/>
        public string DeferredTax
        {
            get
            {
                return this.deferredTaxField;
            }
            set
            {
                this.deferredTaxField = value;
            }
        }

        /// <remarks/>
        public string MeasureUnit
        {
            get
            {
                return this.measureUnitField;
            }
            set
            {
                this.measureUnitField = value;
            }
        }

        /// <remarks/>
        public double UnitsOfMeasurment
        {
            get
            {
                return this.unitsOfMeasurmentField;
            }
            set
            {
                this.unitsOfMeasurmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UnitsOfMeasurmentSpecified
        {
            get
            {
                return this.unitsOfMeasurmentFieldSpecified;
            }
            set
            {
                this.unitsOfMeasurmentFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double LineTotal
        {
            get
            {
                return this.lineTotalField;
            }
            set
            {
                this.lineTotalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LineTotalSpecified
        {
            get
            {
                return this.lineTotalFieldSpecified;
            }
            set
            {
                this.lineTotalFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double TaxPercentagePerRow
        {
            get
            {
                return this.taxPercentagePerRowField;
            }
            set
            {
                this.taxPercentagePerRowField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TaxPercentagePerRowSpecified
        {
            get
            {
                return this.taxPercentagePerRowFieldSpecified;
            }
            set
            {
                this.taxPercentagePerRowFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double TaxTotal
        {
            get
            {
                return this.taxTotalField;
            }
            set
            {
                this.taxTotalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TaxTotalSpecified
        {
            get
            {
                return this.taxTotalFieldSpecified;
            }
            set
            {
                this.taxTotalFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ConsumerSalesForecast
        {
            get
            {
                return this.consumerSalesForecastField;
            }
            set
            {
                this.consumerSalesForecastField = value;
            }
        }

        /// <remarks/>
        public double ExciseAmount
        {
            get
            {
                return this.exciseAmountField;
            }
            set
            {
                this.exciseAmountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ExciseAmountSpecified
        {
            get
            {
                return this.exciseAmountFieldSpecified;
            }
            set
            {
                this.exciseAmountFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string CountryOrg
        {
            get
            {
                return this.countryOrgField;
            }
            set
            {
                this.countryOrgField = value;
            }
        }

        /// <remarks/>
        public string SWW
        {
            get
            {
                return this.sWWField;
            }
            set
            {
                this.sWWField = value;
            }
        }

        /// <remarks/>
        public string TransactionType
        {
            get
            {
                return this.transactionTypeField;
            }
            set
            {
                this.transactionTypeField = value;
            }
        }

        /// <remarks/>
        public string DistributeExpense
        {
            get
            {
                return this.distributeExpenseField;
            }
            set
            {
                this.distributeExpenseField = value;
            }
        }

        /// <remarks/>
        public string ShipToCode
        {
            get
            {
                return this.shipToCodeField;
            }
            set
            {
                this.shipToCodeField = value;
            }
        }

        /// <remarks/>
        public double RowTotalFC
        {
            get
            {
                return this.rowTotalFCField;
            }
            set
            {
                this.rowTotalFCField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RowTotalFCSpecified
        {
            get
            {
                return this.rowTotalFCFieldSpecified;
            }
            set
            {
                this.rowTotalFCFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string CFOPCode
        {
            get
            {
                return this.cFOPCodeField;
            }
            set
            {
                this.cFOPCodeField = value;
            }
        }

        /// <remarks/>
        public string CSTCode
        {
            get
            {
                return this.cSTCodeField;
            }
            set
            {
                this.cSTCodeField = value;
            }
        }

        /// <remarks/>
        public long Usage
        {
            get
            {
                return this.usageField;
            }
            set
            {
                this.usageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UsageSpecified
        {
            get
            {
                return this.usageFieldSpecified;
            }
            set
            {
                this.usageFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string TaxOnly
        {
            get
            {
                return this.taxOnlyField;
            }
            set
            {
                this.taxOnlyField = value;
            }
        }

        /// <remarks/>
        public double UnitPrice
        {
            get
            {
                return this.unitPriceField;
            }
            set
            {
                this.unitPriceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UnitPriceSpecified
        {
            get
            {
                return this.unitPriceFieldSpecified;
            }
            set
            {
                this.unitPriceFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string LineStatus
        {
            get
            {
                return this.lineStatusField;
            }
            set
            {
                this.lineStatusField = value;
            }
        }

        /// <remarks/>
        public string LineType
        {
            get
            {
                return this.lineTypeField;
            }
            set
            {
                this.lineTypeField = value;
            }
        }

        /// <remarks/>
        public string COGSCostingCode
        {
            get
            {
                return this.cOGSCostingCodeField;
            }
            set
            {
                this.cOGSCostingCodeField = value;
            }
        }

        /// <remarks/>
        public string COGSAccountCode
        {
            get
            {
                return this.cOGSAccountCodeField;
            }
            set
            {
                this.cOGSAccountCodeField = value;
            }
        }

        /// <remarks/>
        public string ChangeAssemlyBoMWarehouse
        {
            get
            {
                return this.changeAssemlyBoMWarehouseField;
            }
            set
            {
                this.changeAssemlyBoMWarehouseField = value;
            }
        }

        /// <remarks/>
        public double GrossBuyPrice
        {
            get
            {
                return this.grossBuyPriceField;
            }
            set
            {
                this.grossBuyPriceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GrossBuyPriceSpecified
        {
            get
            {
                return this.grossBuyPriceFieldSpecified;
            }
            set
            {
                this.grossBuyPriceFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long GrossBase
        {
            get
            {
                return this.grossBaseField;
            }
            set
            {
                this.grossBaseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GrossBaseSpecified
        {
            get
            {
                return this.grossBaseFieldSpecified;
            }
            set
            {
                this.grossBaseFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double GrossProfitTotalBasePrice
        {
            get
            {
                return this.grossProfitTotalBasePriceField;
            }
            set
            {
                this.grossProfitTotalBasePriceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GrossProfitTotalBasePriceSpecified
        {
            get
            {
                return this.grossProfitTotalBasePriceFieldSpecified;
            }
            set
            {
                this.grossProfitTotalBasePriceFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string CostingCode2
        {
            get
            {
                return this.costingCode2Field;
            }
            set
            {
                this.costingCode2Field = value;
            }
        }

        /// <remarks/>
        public string CostingCode3
        {
            get
            {
                return this.costingCode3Field;
            }
            set
            {
                this.costingCode3Field = value;
            }
        }

        /// <remarks/>
        public string CostingCode4
        {
            get
            {
                return this.costingCode4Field;
            }
            set
            {
                this.costingCode4Field = value;
            }
        }

        /// <remarks/>
        public string CostingCode5
        {
            get
            {
                return this.costingCode5Field;
            }
            set
            {
                this.costingCode5Field = value;
            }
        }

        /// <remarks/>
        public string ItemDetails
        {
            get
            {
                return this.itemDetailsField;
            }
            set
            {
                this.itemDetailsField = value;
            }
        }

        /// <remarks/>
        public long LocationCode
        {
            get
            {
                return this.locationCodeField;
            }
            set
            {
                this.locationCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LocationCodeSpecified
        {
            get
            {
                return this.locationCodeFieldSpecified;
            }
            set
            {
                this.locationCodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ActualDeliveryDate
        {
            get
            {
                return this.actualDeliveryDateField;
            }
            set
            {
                this.actualDeliveryDateField = value;
            }
        }

        /// <remarks/>
        public string ExLineNo
        {
            get
            {
                return this.exLineNoField;
            }
            set
            {
                this.exLineNoField = value;
            }
        }

        /// <remarks/>
        public string RequiredDate
        {
            get
            {
                return this.requiredDateField;
            }
            set
            {
                this.requiredDateField = value;
            }
        }

        /// <remarks/>
        public double RequiredQuantity
        {
            get
            {
                return this.requiredQuantityField;
            }
            set
            {
                this.requiredQuantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RequiredQuantitySpecified
        {
            get
            {
                return this.requiredQuantityFieldSpecified;
            }
            set
            {
                this.requiredQuantityFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string COGSCostingCode2
        {
            get
            {
                return this.cOGSCostingCode2Field;
            }
            set
            {
                this.cOGSCostingCode2Field = value;
            }
        }

        /// <remarks/>
        public string COGSCostingCode3
        {
            get
            {
                return this.cOGSCostingCode3Field;
            }
            set
            {
                this.cOGSCostingCode3Field = value;
            }
        }

        /// <remarks/>
        public string COGSCostingCode4
        {
            get
            {
                return this.cOGSCostingCode4Field;
            }
            set
            {
                this.cOGSCostingCode4Field = value;
            }
        }

        /// <remarks/>
        public string COGSCostingCode5
        {
            get
            {
                return this.cOGSCostingCode5Field;
            }
            set
            {
                this.cOGSCostingCode5Field = value;
            }
        }

        /// <remarks/>
        public string CSTforIPI
        {
            get
            {
                return this.cSTforIPIField;
            }
            set
            {
                this.cSTforIPIField = value;
            }
        }

        /// <remarks/>
        public string CSTforPIS
        {
            get
            {
                return this.cSTforPISField;
            }
            set
            {
                this.cSTforPISField = value;
            }
        }

        /// <remarks/>
        public string CSTforCOFINS
        {
            get
            {
                return this.cSTforCOFINSField;
            }
            set
            {
                this.cSTforCOFINSField = value;
            }
        }

        /// <remarks/>
        public string CreditOriginCode
        {
            get
            {
                return this.creditOriginCodeField;
            }
            set
            {
                this.creditOriginCodeField = value;
            }
        }

        /// <remarks/>
        public string WithoutInventoryMovement
        {
            get
            {
                return this.withoutInventoryMovementField;
            }
            set
            {
                this.withoutInventoryMovementField = value;
            }
        }

        /// <remarks/>
        public long AgreementNo
        {
            get
            {
                return this.agreementNoField;
            }
            set
            {
                this.agreementNoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AgreementNoSpecified
        {
            get
            {
                return this.agreementNoFieldSpecified;
            }
            set
            {
                this.agreementNoFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long AgreementRowNumber
        {
            get
            {
                return this.agreementRowNumberField;
            }
            set
            {
                this.agreementRowNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AgreementRowNumberSpecified
        {
            get
            {
                return this.agreementRowNumberFieldSpecified;
            }
            set
            {
                this.agreementRowNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ShipToDescription
        {
            get
            {
                return this.shipToDescriptionField;
            }
            set
            {
                this.shipToDescriptionField = value;
            }
        }

        /// <remarks/>
        public long ActualBaseEntry
        {
            get
            {
                return this.actualBaseEntryField;
            }
            set
            {
                this.actualBaseEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ActualBaseEntrySpecified
        {
            get
            {
                return this.actualBaseEntryFieldSpecified;
            }
            set
            {
                this.actualBaseEntryFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long ActualBaseLine
        {
            get
            {
                return this.actualBaseLineField;
            }
            set
            {
                this.actualBaseLineField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ActualBaseLineSpecified
        {
            get
            {
                return this.actualBaseLineFieldSpecified;
            }
            set
            {
                this.actualBaseLineFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long DocEntry
        {
            get
            {
                return this.docEntryField;
            }
            set
            {
                this.docEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocEntrySpecified
        {
            get
            {
                return this.docEntryFieldSpecified;
            }
            set
            {
                this.docEntryFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Surpluses
        {
            get
            {
                return this.surplusesField;
            }
            set
            {
                this.surplusesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SurplusesSpecified
        {
            get
            {
                return this.surplusesFieldSpecified;
            }
            set
            {
                this.surplusesFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double DefectAndBreakup
        {
            get
            {
                return this.defectAndBreakupField;
            }
            set
            {
                this.defectAndBreakupField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DefectAndBreakupSpecified
        {
            get
            {
                return this.defectAndBreakupFieldSpecified;
            }
            set
            {
                this.defectAndBreakupFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Shortages
        {
            get
            {
                return this.shortagesField;
            }
            set
            {
                this.shortagesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShortagesSpecified
        {
            get
            {
                return this.shortagesFieldSpecified;
            }
            set
            {
                this.shortagesFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ConsiderQuantity
        {
            get
            {
                return this.considerQuantityField;
            }
            set
            {
                this.considerQuantityField = value;
            }
        }

        /// <remarks/>
        public string PartialRetirement
        {
            get
            {
                return this.partialRetirementField;
            }
            set
            {
                this.partialRetirementField = value;
            }
        }

        /// <remarks/>
        public double RetirementQuantity
        {
            get
            {
                return this.retirementQuantityField;
            }
            set
            {
                this.retirementQuantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RetirementQuantitySpecified
        {
            get
            {
                return this.retirementQuantityFieldSpecified;
            }
            set
            {
                this.retirementQuantityFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double RetirementAPC
        {
            get
            {
                return this.retirementAPCField;
            }
            set
            {
                this.retirementAPCField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RetirementAPCSpecified
        {
            get
            {
                return this.retirementAPCFieldSpecified;
            }
            set
            {
                this.retirementAPCFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long UoMEntry
        {
            get
            {
                return this.uoMEntryField;
            }
            set
            {
                this.uoMEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UoMEntrySpecified
        {
            get
            {
                return this.uoMEntryFieldSpecified;
            }
            set
            {
                this.uoMEntryFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double InventoryQuantity
        {
            get
            {
                return this.inventoryQuantityField;
            }
            set
            {
                this.inventoryQuantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool InventoryQuantitySpecified
        {
            get
            {
                return this.inventoryQuantityFieldSpecified;
            }
            set
            {
                this.inventoryQuantityFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Incoterms
        {
            get
            {
                return this.incotermsField;
            }
            set
            {
                this.incotermsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IncotermsSpecified
        {
            get
            {
                return this.incotermsFieldSpecified;
            }
            set
            {
                this.incotermsFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long TransportMode
        {
            get
            {
                return this.transportModeField;
            }
            set
            {
                this.transportModeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TransportModeSpecified
        {
            get
            {
                return this.transportModeFieldSpecified;
            }
            set
            {
                this.transportModeFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class LineTaxJurisdictionsField
    {

        private string jurisdictionCodeField;

        private long jurisdictionTypeField;

        private bool jurisdictionTypeFieldSpecified;

        private double taxAmountField;

        private bool taxAmountFieldSpecified;

        private long docEntryField;

        private bool docEntryFieldSpecified;

        private long lineNumberField;

        private bool lineNumberFieldSpecified;

        private long rowSequenceField;

        private bool rowSequenceFieldSpecified;

        private string u_TX_AdjustedField;

        private double u_TX_BaseSumField;

        private bool u_TX_BaseSumFieldSpecified;

        private double u_TX_TaxRateField;

        private bool u_TX_TaxRateFieldSpecified;

        private double u_TX_TaxSumField;

        private bool u_TX_TaxSumFieldSpecified;

        private double u_TX_U_ExcAmtLField;

        private bool u_TX_U_ExcAmtLFieldSpecified;

        private double u_TX_U_OthAmtLField;

        private bool u_TX_U_OthAmtLFieldSpecified;

        private double u_BaseField;

        private bool u_BaseFieldSpecified;

        private double u_IsentoField;

        private bool u_IsentoFieldSpecified;

        private double u_OutrosField;

        private bool u_OutrosFieldSpecified;

        private double u_MinimoField;

        private bool u_MinimoFieldSpecified;

        private double u_UnidadesField;

        private bool u_UnidadesFieldSpecified;

        private string u_MedidaField;

        private double u_PrecoMinField;

        private bool u_PrecoMinFieldSpecified;

        private string u_MoedaField;

        private double u_LucroField;

        private bool u_LucroFieldSpecified;

        private double u_Reducao1Field;

        private bool u_Reducao1FieldSpecified;

        private double u_Reducao2Field;

        private bool u_Reducao2FieldSpecified;

        private double u_ReduICMSField;

        private bool u_ReduICMSFieldSpecified;

        private double u_PrecoFixField;

        private bool u_PrecoFixFieldSpecified;

        private double u_FatorPrcField;

        private bool u_FatorPrcFieldSpecified;

        private double u_ExcAmtLField;

        private bool u_ExcAmtLFieldSpecified;

        private double u_ExcAmtFField;

        private bool u_ExcAmtFFieldSpecified;

        private double u_ExcAmtSField;

        private bool u_ExcAmtSFieldSpecified;

        private double u_OthAmtLField;

        private bool u_OthAmtLFieldSpecified;

        private double u_OthAmtFField;

        private bool u_OthAmtFFieldSpecified;

        private double u_OthAmtSField;

        private bool u_OthAmtSFieldSpecified;

        private double u_TotalBLField;

        private bool u_TotalBLFieldSpecified;

        private double u_TotalBFField;

        private bool u_TotalBFFieldSpecified;

        private double u_TotalBSField;

        private bool u_TotalBSFieldSpecified;

        /// <remarks/>
        public string JurisdictionCode
        {
            get
            {
                return this.jurisdictionCodeField;
            }
            set
            {
                this.jurisdictionCodeField = value;
            }
        }

        /// <remarks/>
        public long JurisdictionType
        {
            get
            {
                return this.jurisdictionTypeField;
            }
            set
            {
                this.jurisdictionTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool JurisdictionTypeSpecified
        {
            get
            {
                return this.jurisdictionTypeFieldSpecified;
            }
            set
            {
                this.jurisdictionTypeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double TaxAmount
        {
            get
            {
                return this.taxAmountField;
            }
            set
            {
                this.taxAmountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TaxAmountSpecified
        {
            get
            {
                return this.taxAmountFieldSpecified;
            }
            set
            {
                this.taxAmountFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long DocEntry
        {
            get
            {
                return this.docEntryField;
            }
            set
            {
                this.docEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocEntrySpecified
        {
            get
            {
                return this.docEntryFieldSpecified;
            }
            set
            {
                this.docEntryFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long LineNumber
        {
            get
            {
                return this.lineNumberField;
            }
            set
            {
                this.lineNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LineNumberSpecified
        {
            get
            {
                return this.lineNumberFieldSpecified;
            }
            set
            {
                this.lineNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long RowSequence
        {
            get
            {
                return this.rowSequenceField;
            }
            set
            {
                this.rowSequenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RowSequenceSpecified
        {
            get
            {
                return this.rowSequenceFieldSpecified;
            }
            set
            {
                this.rowSequenceFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string U_TX_Adjusted
        {
            get
            {
                return this.u_TX_AdjustedField;
            }
            set
            {
                this.u_TX_AdjustedField = value;
            }
        }

        /// <remarks/>
        public double U_TX_BaseSum
        {
            get
            {
                return this.u_TX_BaseSumField;
            }
            set
            {
                this.u_TX_BaseSumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_TX_BaseSumSpecified
        {
            get
            {
                return this.u_TX_BaseSumFieldSpecified;
            }
            set
            {
                this.u_TX_BaseSumFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_TX_TaxRate
        {
            get
            {
                return this.u_TX_TaxRateField;
            }
            set
            {
                this.u_TX_TaxRateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_TX_TaxRateSpecified
        {
            get
            {
                return this.u_TX_TaxRateFieldSpecified;
            }
            set
            {
                this.u_TX_TaxRateFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_TX_TaxSum
        {
            get
            {
                return this.u_TX_TaxSumField;
            }
            set
            {
                this.u_TX_TaxSumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_TX_TaxSumSpecified
        {
            get
            {
                return this.u_TX_TaxSumFieldSpecified;
            }
            set
            {
                this.u_TX_TaxSumFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_TX_U_ExcAmtL
        {
            get
            {
                return this.u_TX_U_ExcAmtLField;
            }
            set
            {
                this.u_TX_U_ExcAmtLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_TX_U_ExcAmtLSpecified
        {
            get
            {
                return this.u_TX_U_ExcAmtLFieldSpecified;
            }
            set
            {
                this.u_TX_U_ExcAmtLFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_TX_U_OthAmtL
        {
            get
            {
                return this.u_TX_U_OthAmtLField;
            }
            set
            {
                this.u_TX_U_OthAmtLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_TX_U_OthAmtLSpecified
        {
            get
            {
                return this.u_TX_U_OthAmtLFieldSpecified;
            }
            set
            {
                this.u_TX_U_OthAmtLFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_Base
        {
            get
            {
                return this.u_BaseField;
            }
            set
            {
                this.u_BaseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_BaseSpecified
        {
            get
            {
                return this.u_BaseFieldSpecified;
            }
            set
            {
                this.u_BaseFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_Isento
        {
            get
            {
                return this.u_IsentoField;
            }
            set
            {
                this.u_IsentoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_IsentoSpecified
        {
            get
            {
                return this.u_IsentoFieldSpecified;
            }
            set
            {
                this.u_IsentoFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_Outros
        {
            get
            {
                return this.u_OutrosField;
            }
            set
            {
                this.u_OutrosField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_OutrosSpecified
        {
            get
            {
                return this.u_OutrosFieldSpecified;
            }
            set
            {
                this.u_OutrosFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_Minimo
        {
            get
            {
                return this.u_MinimoField;
            }
            set
            {
                this.u_MinimoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_MinimoSpecified
        {
            get
            {
                return this.u_MinimoFieldSpecified;
            }
            set
            {
                this.u_MinimoFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_Unidades
        {
            get
            {
                return this.u_UnidadesField;
            }
            set
            {
                this.u_UnidadesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_UnidadesSpecified
        {
            get
            {
                return this.u_UnidadesFieldSpecified;
            }
            set
            {
                this.u_UnidadesFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string U_Medida
        {
            get
            {
                return this.u_MedidaField;
            }
            set
            {
                this.u_MedidaField = value;
            }
        }

        /// <remarks/>
        public double U_PrecoMin
        {
            get
            {
                return this.u_PrecoMinField;
            }
            set
            {
                this.u_PrecoMinField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_PrecoMinSpecified
        {
            get
            {
                return this.u_PrecoMinFieldSpecified;
            }
            set
            {
                this.u_PrecoMinFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string U_Moeda
        {
            get
            {
                return this.u_MoedaField;
            }
            set
            {
                this.u_MoedaField = value;
            }
        }

        /// <remarks/>
        public double U_Lucro
        {
            get
            {
                return this.u_LucroField;
            }
            set
            {
                this.u_LucroField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_LucroSpecified
        {
            get
            {
                return this.u_LucroFieldSpecified;
            }
            set
            {
                this.u_LucroFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_Reducao1
        {
            get
            {
                return this.u_Reducao1Field;
            }
            set
            {
                this.u_Reducao1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_Reducao1Specified
        {
            get
            {
                return this.u_Reducao1FieldSpecified;
            }
            set
            {
                this.u_Reducao1FieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_Reducao2
        {
            get
            {
                return this.u_Reducao2Field;
            }
            set
            {
                this.u_Reducao2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_Reducao2Specified
        {
            get
            {
                return this.u_Reducao2FieldSpecified;
            }
            set
            {
                this.u_Reducao2FieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_ReduICMS
        {
            get
            {
                return this.u_ReduICMSField;
            }
            set
            {
                this.u_ReduICMSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_ReduICMSSpecified
        {
            get
            {
                return this.u_ReduICMSFieldSpecified;
            }
            set
            {
                this.u_ReduICMSFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_PrecoFix
        {
            get
            {
                return this.u_PrecoFixField;
            }
            set
            {
                this.u_PrecoFixField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_PrecoFixSpecified
        {
            get
            {
                return this.u_PrecoFixFieldSpecified;
            }
            set
            {
                this.u_PrecoFixFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_FatorPrc
        {
            get
            {
                return this.u_FatorPrcField;
            }
            set
            {
                this.u_FatorPrcField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_FatorPrcSpecified
        {
            get
            {
                return this.u_FatorPrcFieldSpecified;
            }
            set
            {
                this.u_FatorPrcFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_ExcAmtL
        {
            get
            {
                return this.u_ExcAmtLField;
            }
            set
            {
                this.u_ExcAmtLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_ExcAmtLSpecified
        {
            get
            {
                return this.u_ExcAmtLFieldSpecified;
            }
            set
            {
                this.u_ExcAmtLFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_ExcAmtF
        {
            get
            {
                return this.u_ExcAmtFField;
            }
            set
            {
                this.u_ExcAmtFField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_ExcAmtFSpecified
        {
            get
            {
                return this.u_ExcAmtFFieldSpecified;
            }
            set
            {
                this.u_ExcAmtFFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_ExcAmtS
        {
            get
            {
                return this.u_ExcAmtSField;
            }
            set
            {
                this.u_ExcAmtSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_ExcAmtSSpecified
        {
            get
            {
                return this.u_ExcAmtSFieldSpecified;
            }
            set
            {
                this.u_ExcAmtSFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_OthAmtL
        {
            get
            {
                return this.u_OthAmtLField;
            }
            set
            {
                this.u_OthAmtLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_OthAmtLSpecified
        {
            get
            {
                return this.u_OthAmtLFieldSpecified;
            }
            set
            {
                this.u_OthAmtLFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_OthAmtF
        {
            get
            {
                return this.u_OthAmtFField;
            }
            set
            {
                this.u_OthAmtFField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_OthAmtFSpecified
        {
            get
            {
                return this.u_OthAmtFFieldSpecified;
            }
            set
            {
                this.u_OthAmtFFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_OthAmtS
        {
            get
            {
                return this.u_OthAmtSField;
            }
            set
            {
                this.u_OthAmtSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_OthAmtSSpecified
        {
            get
            {
                return this.u_OthAmtSFieldSpecified;
            }
            set
            {
                this.u_OthAmtSFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_TotalBL
        {
            get
            {
                return this.u_TotalBLField;
            }
            set
            {
                this.u_TotalBLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_TotalBLSpecified
        {
            get
            {
                return this.u_TotalBLFieldSpecified;
            }
            set
            {
                this.u_TotalBLFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_TotalBF
        {
            get
            {
                return this.u_TotalBFField;
            }
            set
            {
                this.u_TotalBFField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_TotalBFSpecified
        {
            get
            {
                return this.u_TotalBFFieldSpecified;
            }
            set
            {
                this.u_TotalBFFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double U_TotalBS
        {
            get
            {
                return this.u_TotalBSField;
            }
            set
            {
                this.u_TotalBSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool U_TotalBSSpecified
        {
            get
            {
                return this.u_TotalBSFieldSpecified;
            }
            set
            {
                this.u_TotalBSFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SerialNumbersField
    {

        private string manufacturerSerialNumberField;

        private string internalSerialNumberField;

        private string expiryDateField;

        private string manufactureDateField;

        private string receptionDateField;

        private string warrantyStartField;

        private string warrantyEndField;

        private string locationField;

        private string notesField;

        private string batchIDField;

        private long systemSerialNumberField;

        private bool systemSerialNumberFieldSpecified;

        private long baseLineNumberField;

        private bool baseLineNumberFieldSpecified;

        private double quantityField;

        private bool quantityFieldSpecified;

        /// <remarks/>
        public string ManufacturerSerialNumber
        {
            get
            {
                return this.manufacturerSerialNumberField;
            }
            set
            {
                this.manufacturerSerialNumberField = value;
            }
        }

        /// <remarks/>
        public string InternalSerialNumber
        {
            get
            {
                return this.internalSerialNumberField;
            }
            set
            {
                this.internalSerialNumberField = value;
            }
        }

        /// <remarks/>
        public string ExpiryDate
        {
            get
            {
                return this.expiryDateField;
            }
            set
            {
                this.expiryDateField = value;
            }
        }

        /// <remarks/>
        public string ManufactureDate
        {
            get
            {
                return this.manufactureDateField;
            }
            set
            {
                this.manufactureDateField = value;
            }
        }

        /// <remarks/>
        public string ReceptionDate
        {
            get
            {
                return this.receptionDateField;
            }
            set
            {
                this.receptionDateField = value;
            }
        }

        /// <remarks/>
        public string WarrantyStart
        {
            get
            {
                return this.warrantyStartField;
            }
            set
            {
                this.warrantyStartField = value;
            }
        }

        /// <remarks/>
        public string WarrantyEnd
        {
            get
            {
                return this.warrantyEndField;
            }
            set
            {
                this.warrantyEndField = value;
            }
        }

        /// <remarks/>
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        public string Notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }

        /// <remarks/>
        public string BatchID
        {
            get
            {
                return this.batchIDField;
            }
            set
            {
                this.batchIDField = value;
            }
        }

        /// <remarks/>
        public long SystemSerialNumber
        {
            get
            {
                return this.systemSerialNumberField;
            }
            set
            {
                this.systemSerialNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SystemSerialNumberSpecified
        {
            get
            {
                return this.systemSerialNumberFieldSpecified;
            }
            set
            {
                this.systemSerialNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long BaseLineNumber
        {
            get
            {
                return this.baseLineNumberField;
            }
            set
            {
                this.baseLineNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BaseLineNumberSpecified
        {
            get
            {
                return this.baseLineNumberFieldSpecified;
            }
            set
            {
                this.baseLineNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool QuantitySpecified
        {
            get
            {
                return this.quantityFieldSpecified;
            }
            set
            {
                this.quantityFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class BatchNumbersField
    {

        private string batchNumberField;

        private string manufacturerSerialNumberField;

        private string internalSerialNumberField;

        private string expiryDateField;

        private string manufacturingDateField;

        private string addmisionDateField;

        private string locationField;

        private string notesField;

        private double quantityField;

        private bool quantityFieldSpecified;

        private long baseLineNumberField;

        private bool baseLineNumberFieldSpecified;

        /// <remarks/>
        public string BatchNumber
        {
            get
            {
                return this.batchNumberField;
            }
            set
            {
                this.batchNumberField = value;
            }
        }

        /// <remarks/>
        public string ManufacturerSerialNumber
        {
            get
            {
                return this.manufacturerSerialNumberField;
            }
            set
            {
                this.manufacturerSerialNumberField = value;
            }
        }

        /// <remarks/>
        public string InternalSerialNumber
        {
            get
            {
                return this.internalSerialNumberField;
            }
            set
            {
                this.internalSerialNumberField = value;
            }
        }

        /// <remarks/>
        public string ExpiryDate
        {
            get
            {
                return this.expiryDateField;
            }
            set
            {
                this.expiryDateField = value;
            }
        }

        /// <remarks/>
        public string ManufacturingDate
        {
            get
            {
                return this.manufacturingDateField;
            }
            set
            {
                this.manufacturingDateField = value;
            }
        }

        /// <remarks/>
        public string AddmisionDate
        {
            get
            {
                return this.addmisionDateField;
            }
            set
            {
                this.addmisionDateField = value;
            }
        }

        /// <remarks/>
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        public string Notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }

        /// <remarks/>
        public double Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool QuantitySpecified
        {
            get
            {
                return this.quantityFieldSpecified;
            }
            set
            {
                this.quantityFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long BaseLineNumber
        {
            get
            {
                return this.baseLineNumberField;
            }
            set
            {
                this.baseLineNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BaseLineNumberSpecified
        {
            get
            {
                return this.baseLineNumberFieldSpecified;
            }
            set
            {
                this.baseLineNumberFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DocumentLinesBinAllocationsField
    {

        private long binAbsEntryField;

        private bool binAbsEntryFieldSpecified;

        private double quantityField;

        private bool quantityFieldSpecified;

        private string allowNegativeQuantityField;

        private long serialAndBatchNumbersBaseLineField;

        private bool serialAndBatchNumbersBaseLineFieldSpecified;

        private long baseLineNumberField;

        private bool baseLineNumberFieldSpecified;

        /// <remarks/>
        public long BinAbsEntry
        {
            get
            {
                return this.binAbsEntryField;
            }
            set
            {
                this.binAbsEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BinAbsEntrySpecified
        {
            get
            {
                return this.binAbsEntryFieldSpecified;
            }
            set
            {
                this.binAbsEntryFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool QuantitySpecified
        {
            get
            {
                return this.quantityFieldSpecified;
            }
            set
            {
                this.quantityFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string AllowNegativeQuantity
        {
            get
            {
                return this.allowNegativeQuantityField;
            }
            set
            {
                this.allowNegativeQuantityField = value;
            }
        }

        /// <remarks/>
        public long SerialAndBatchNumbersBaseLine
        {
            get
            {
                return this.serialAndBatchNumbersBaseLineField;
            }
            set
            {
                this.serialAndBatchNumbersBaseLineField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SerialAndBatchNumbersBaseLineSpecified
        {
            get
            {
                return this.serialAndBatchNumbersBaseLineFieldSpecified;
            }
            set
            {
                this.serialAndBatchNumbersBaseLineFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long BaseLineNumber
        {
            get
            {
                return this.baseLineNumberField;
            }
            set
            {
                this.baseLineNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BaseLineNumberSpecified
        {
            get
            {
                return this.baseLineNumberFieldSpecified;
            }
            set
            {
                this.baseLineNumberFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TaxExtensionField
    {

        private string taxId0Field;

        private string taxId1Field;

        private string taxId2Field;

        private string taxId3Field;

        private string taxId4Field;

        private string taxId5Field;

        private string taxId6Field;

        private string taxId7Field;

        private string taxId8Field;

        private string taxId9Field;

        private string stateField;

        private string countyField;

        private string incotermsField;

        private string vehicleField;

        private string vehicleStateField;

        private string nFRefField;

        private string carrierField;

        private long packQuantityField;

        private bool packQuantityFieldSpecified;

        private string packDescriptionField;

        private string brandField;

        private long shipUnitNoField;

        private bool shipUnitNoFieldSpecified;

        private double netWeightField;

        private bool netWeightFieldSpecified;

        private double grossWeightField;

        private bool grossWeightFieldSpecified;

        private string streetSField;

        private string blockSField;

        private string buildingSField;

        private string citySField;

        private string zipCodeSField;

        private string countySField;

        private string stateSField;

        private string countrySField;

        private string streetBField;

        private string blockBField;

        private string buildingBField;

        private string cityBField;

        private string zipCodeBField;

        private string countyBField;

        private string stateBField;

        private string countryBField;

        private string importOrExportField;

        private long mainUsageField;

        private bool mainUsageFieldSpecified;

        private string globalLocationNumberSField;

        private string globalLocationNumberBField;

        /// <remarks/>
        public string TaxId0
        {
            get
            {
                return this.taxId0Field;
            }
            set
            {
                this.taxId0Field = value;
            }
        }

        /// <remarks/>
        public string TaxId1
        {
            get
            {
                return this.taxId1Field;
            }
            set
            {
                this.taxId1Field = value;
            }
        }

        /// <remarks/>
        public string TaxId2
        {
            get
            {
                return this.taxId2Field;
            }
            set
            {
                this.taxId2Field = value;
            }
        }

        /// <remarks/>
        public string TaxId3
        {
            get
            {
                return this.taxId3Field;
            }
            set
            {
                this.taxId3Field = value;
            }
        }

        /// <remarks/>
        public string TaxId4
        {
            get
            {
                return this.taxId4Field;
            }
            set
            {
                this.taxId4Field = value;
            }
        }

        /// <remarks/>
        public string TaxId5
        {
            get
            {
                return this.taxId5Field;
            }
            set
            {
                this.taxId5Field = value;
            }
        }

        /// <remarks/>
        public string TaxId6
        {
            get
            {
                return this.taxId6Field;
            }
            set
            {
                this.taxId6Field = value;
            }
        }

        /// <remarks/>
        public string TaxId7
        {
            get
            {
                return this.taxId7Field;
            }
            set
            {
                this.taxId7Field = value;
            }
        }

        /// <remarks/>
        public string TaxId8
        {
            get
            {
                return this.taxId8Field;
            }
            set
            {
                this.taxId8Field = value;
            }
        }

        /// <remarks/>
        public string TaxId9
        {
            get
            {
                return this.taxId9Field;
            }
            set
            {
                this.taxId9Field = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string County
        {
            get
            {
                return this.countyField;
            }
            set
            {
                this.countyField = value;
            }
        }

        /// <remarks/>
        public string Incoterms
        {
            get
            {
                return this.incotermsField;
            }
            set
            {
                this.incotermsField = value;
            }
        }

        /// <remarks/>
        public string Vehicle
        {
            get
            {
                return this.vehicleField;
            }
            set
            {
                this.vehicleField = value;
            }
        }

        /// <remarks/>
        public string VehicleState
        {
            get
            {
                return this.vehicleStateField;
            }
            set
            {
                this.vehicleStateField = value;
            }
        }

        /// <remarks/>
        public string NFRef
        {
            get
            {
                return this.nFRefField;
            }
            set
            {
                this.nFRefField = value;
            }
        }

        /// <remarks/>
        public string Carrier
        {
            get
            {
                return this.carrierField;
            }
            set
            {
                this.carrierField = value;
            }
        }

        /// <remarks/>
        public long PackQuantity
        {
            get
            {
                return this.packQuantityField;
            }
            set
            {
                this.packQuantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PackQuantitySpecified
        {
            get
            {
                return this.packQuantityFieldSpecified;
            }
            set
            {
                this.packQuantityFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string PackDescription
        {
            get
            {
                return this.packDescriptionField;
            }
            set
            {
                this.packDescriptionField = value;
            }
        }

        /// <remarks/>
        public string Brand
        {
            get
            {
                return this.brandField;
            }
            set
            {
                this.brandField = value;
            }
        }

        /// <remarks/>
        public long ShipUnitNo
        {
            get
            {
                return this.shipUnitNoField;
            }
            set
            {
                this.shipUnitNoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShipUnitNoSpecified
        {
            get
            {
                return this.shipUnitNoFieldSpecified;
            }
            set
            {
                this.shipUnitNoFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double NetWeight
        {
            get
            {
                return this.netWeightField;
            }
            set
            {
                this.netWeightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NetWeightSpecified
        {
            get
            {
                return this.netWeightFieldSpecified;
            }
            set
            {
                this.netWeightFieldSpecified = value;
            }
        }

        /// <remarks/>
        public double GrossWeight
        {
            get
            {
                return this.grossWeightField;
            }
            set
            {
                this.grossWeightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GrossWeightSpecified
        {
            get
            {
                return this.grossWeightFieldSpecified;
            }
            set
            {
                this.grossWeightFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string StreetS
        {
            get
            {
                return this.streetSField;
            }
            set
            {
                this.streetSField = value;
            }
        }

        /// <remarks/>
        public string BlockS
        {
            get
            {
                return this.blockSField;
            }
            set
            {
                this.blockSField = value;
            }
        }

        /// <remarks/>
        public string BuildingS
        {
            get
            {
                return this.buildingSField;
            }
            set
            {
                this.buildingSField = value;
            }
        }

        /// <remarks/>
        public string CityS
        {
            get
            {
                return this.citySField;
            }
            set
            {
                this.citySField = value;
            }
        }

        /// <remarks/>
        public string ZipCodeS
        {
            get
            {
                return this.zipCodeSField;
            }
            set
            {
                this.zipCodeSField = value;
            }
        }

        /// <remarks/>
        public string CountyS
        {
            get
            {
                return this.countySField;
            }
            set
            {
                this.countySField = value;
            }
        }

        /// <remarks/>
        public string StateS
        {
            get
            {
                return this.stateSField;
            }
            set
            {
                this.stateSField = value;
            }
        }

        /// <remarks/>
        public string CountryS
        {
            get
            {
                return this.countrySField;
            }
            set
            {
                this.countrySField = value;
            }
        }

        /// <remarks/>
        public string StreetB
        {
            get
            {
                return this.streetBField;
            }
            set
            {
                this.streetBField = value;
            }
        }

        /// <remarks/>
        public string BlockB
        {
            get
            {
                return this.blockBField;
            }
            set
            {
                this.blockBField = value;
            }
        }

        /// <remarks/>
        public string BuildingB
        {
            get
            {
                return this.buildingBField;
            }
            set
            {
                this.buildingBField = value;
            }
        }

        /// <remarks/>
        public string CityB
        {
            get
            {
                return this.cityBField;
            }
            set
            {
                this.cityBField = value;
            }
        }

        /// <remarks/>
        public string ZipCodeB
        {
            get
            {
                return this.zipCodeBField;
            }
            set
            {
                this.zipCodeBField = value;
            }
        }

        /// <remarks/>
        public string CountyB
        {
            get
            {
                return this.countyBField;
            }
            set
            {
                this.countyBField = value;
            }
        }

        /// <remarks/>
        public string StateB
        {
            get
            {
                return this.stateBField;
            }
            set
            {
                this.stateBField = value;
            }
        }

        /// <remarks/>
        public string CountryB
        {
            get
            {
                return this.countryBField;
            }
            set
            {
                this.countryBField = value;
            }
        }

        /// <remarks/>
        public string ImportOrExport
        {
            get
            {
                return this.importOrExportField;
            }
            set
            {
                this.importOrExportField = value;
            }
        }

        /// <remarks/>
        public long MainUsage
        {
            get
            {
                return this.mainUsageField;
            }
            set
            {
                this.mainUsageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MainUsageSpecified
        {
            get
            {
                return this.mainUsageFieldSpecified;
            }
            set
            {
                this.mainUsageFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string GlobalLocationNumberS
        {
            get
            {
                return this.globalLocationNumberSField;
            }
            set
            {
                this.globalLocationNumberSField = value;
            }
        }

        /// <remarks/>
        public string GlobalLocationNumberB
        {
            get
            {
                return this.globalLocationNumberBField;
            }
            set
            {
                this.globalLocationNumberBField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AddressExtensionField
    {

        private string shipToStreetField;

        private string shipToStreetNoField;

        private string shipToBlockField;

        private string shipToBuildingField;

        private string shipToCityField;

        private string shipToZipCodeField;

        private string shipToCountyField;

        private string shipToStateField;

        private string shipToCountryField;

        private string shipToAddressTypeField;

        private string billToStreetField;

        private string billToStreetNoField;

        private string billToBlockField;

        private string billToBuildingField;

        private string billToCityField;

        private string billToZipCodeField;

        private string billToCountyField;

        private string billToStateField;

        private string billToCountryField;

        private string billToAddressTypeField;

        private string shipToGlobalLocationNumberField;

        private string billToGlobalLocationNumberField;

        private string shipToAddress2Field;

        private string shipToAddress3Field;

        private string billToAddress2Field;

        private string billToAddress3Field;

        /// <remarks/>
        public string ShipToStreet
        {
            get
            {
                return this.shipToStreetField;
            }
            set
            {
                this.shipToStreetField = value;
            }
        }

        /// <remarks/>
        public string ShipToStreetNo
        {
            get
            {
                return this.shipToStreetNoField;
            }
            set
            {
                this.shipToStreetNoField = value;
            }
        }

        /// <remarks/>
        public string ShipToBlock
        {
            get
            {
                return this.shipToBlockField;
            }
            set
            {
                this.shipToBlockField = value;
            }
        }

        /// <remarks/>
        public string ShipToBuilding
        {
            get
            {
                return this.shipToBuildingField;
            }
            set
            {
                this.shipToBuildingField = value;
            }
        }

        /// <remarks/>
        public string ShipToCity
        {
            get
            {
                return this.shipToCityField;
            }
            set
            {
                this.shipToCityField = value;
            }
        }

        /// <remarks/>
        public string ShipToZipCode
        {
            get
            {
                return this.shipToZipCodeField;
            }
            set
            {
                this.shipToZipCodeField = value;
            }
        }

        /// <remarks/>
        public string ShipToCounty
        {
            get
            {
                return this.shipToCountyField;
            }
            set
            {
                this.shipToCountyField = value;
            }
        }

        /// <remarks/>
        public string ShipToState
        {
            get
            {
                return this.shipToStateField;
            }
            set
            {
                this.shipToStateField = value;
            }
        }

        /// <remarks/>
        public string ShipToCountry
        {
            get
            {
                return this.shipToCountryField;
            }
            set
            {
                this.shipToCountryField = value;
            }
        }

        /// <remarks/>
        public string ShipToAddressType
        {
            get
            {
                return this.shipToAddressTypeField;
            }
            set
            {
                this.shipToAddressTypeField = value;
            }
        }

        /// <remarks/>
        public string BillToStreet
        {
            get
            {
                return this.billToStreetField;
            }
            set
            {
                this.billToStreetField = value;
            }
        }

        /// <remarks/>
        public string BillToStreetNo
        {
            get
            {
                return this.billToStreetNoField;
            }
            set
            {
                this.billToStreetNoField = value;
            }
        }

        /// <remarks/>
        public string BillToBlock
        {
            get
            {
                return this.billToBlockField;
            }
            set
            {
                this.billToBlockField = value;
            }
        }

        /// <remarks/>
        public string BillToBuilding
        {
            get
            {
                return this.billToBuildingField;
            }
            set
            {
                this.billToBuildingField = value;
            }
        }

        /// <remarks/>
        public string BillToCity
        {
            get
            {
                return this.billToCityField;
            }
            set
            {
                this.billToCityField = value;
            }
        }

        /// <remarks/>
        public string BillToZipCode
        {
            get
            {
                return this.billToZipCodeField;
            }
            set
            {
                this.billToZipCodeField = value;
            }
        }

        /// <remarks/>
        public string BillToCounty
        {
            get
            {
                return this.billToCountyField;
            }
            set
            {
                this.billToCountyField = value;
            }
        }

        /// <remarks/>
        public string BillToState
        {
            get
            {
                return this.billToStateField;
            }
            set
            {
                this.billToStateField = value;
            }
        }

        /// <remarks/>
        public string BillToCountry
        {
            get
            {
                return this.billToCountryField;
            }
            set
            {
                this.billToCountryField = value;
            }
        }

        /// <remarks/>
        public string BillToAddressType
        {
            get
            {
                return this.billToAddressTypeField;
            }
            set
            {
                this.billToAddressTypeField = value;
            }
        }

        /// <remarks/>
        public string ShipToGlobalLocationNumber
        {
            get
            {
                return this.shipToGlobalLocationNumberField;
            }
            set
            {
                this.shipToGlobalLocationNumberField = value;
            }
        }

        /// <remarks/>
        public string BillToGlobalLocationNumber
        {
            get
            {
                return this.billToGlobalLocationNumberField;
            }
            set
            {
                this.billToGlobalLocationNumberField = value;
            }
        }

        /// <remarks/>
        public string ShipToAddress2
        {
            get
            {
                return this.shipToAddress2Field;
            }
            set
            {
                this.shipToAddress2Field = value;
            }
        }

        /// <remarks/>
        public string ShipToAddress3
        {
            get
            {
                return this.shipToAddress3Field;
            }
            set
            {
                this.shipToAddress3Field = value;
            }
        }

        /// <remarks/>
        public string BillToAddress2
        {
            get
            {
                return this.billToAddress2Field;
            }
            set
            {
                this.billToAddress2Field = value;
            }
        }

        /// <remarks/>
        public string BillToAddress3
        {
            get
            {
                return this.billToAddress3Field;
            }
            set
            {
                this.billToAddress3Field = value;
            }
        }
    }
}
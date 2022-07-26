namespace Dncy.Tools.Excel
{
    public class Schemas
    {
        /// <summary>
        /// Extention Schema types
        /// </summary>
        internal const string schemaXmlExtension = "application/xml";
        internal const string schemaRelsExtension = "application/vnd.openxmlformats-package.relationships+xml";
        /// <summary>
        /// Main Xml schema name
        /// </summary>
        internal const string schemaWorkbook = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";

        internal const string schemaMain = @"http://schemas.openxmlformats.org/spreadsheetml/2006/main";

        /// <summary>
        /// Relationship schema name
        /// </summary>
        internal const string schemaRelationships = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships";


        internal const string schemaDrawings = @"http://schemas.openxmlformats.org/drawingml/2006/main";
        internal const string schemaSheetDrawings = @"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing";
        internal const string schemaMarkupCompatibility = @"http://schemas.openxmlformats.org/markup-compatibility/2006";
        internal const string schemaChart14 = "http://schemas.microsoft.com/office/drawing/2007/8/2/chart";

        internal const string schemaMicrosoftVml = @"urn:schemas-microsoft-com:vml";
        internal const string schemaMicrosoftOffice = "urn:schemas-microsoft-com:office:office";
        internal const string schemaMicrosoftExcel = "urn:schemas-microsoft-com:office:excel";

        internal const string schemaChart = @"http://schemas.openxmlformats.org/drawingml/2006/chart";
        internal const string schemaHyperlink = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";
        internal const string schemaComment = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/comments";
        internal const string schemaImage = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
        internal const string schemaThemeRelationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme";

        internal const string schemaChartStyle = "http://schemas.microsoft.com/office/drawing/2012/chartStyle";

        //Chart styling
        internal const string schemaChartStyleRelationships = "http://schemas.microsoft.com/office/2011/relationships/chartStyle";
        internal const string schemaChartColorStyleRelationships = "http://schemas.microsoft.com/office/2011/relationships/chartColorStyle";

        internal const string schemaThemeOverrideRelationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/themeOverride";

        //Office properties
        internal const string schemaCore = @"http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
        internal const string schemaExtended = @"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
        internal const string schemaCustom = @"http://schemas.openxmlformats.org/officeDocument/2006/custom-properties";
        internal const string schemaDc = @"http://purl.org/dc/elements/1.1/";
        internal const string schemaDcTerms = @"http://purl.org/dc/terms/";
        internal const string schemaDcmiType = @"http://purl.org/dc/dcmitype/";
        internal const string schemaXsi = @"http://www.w3.org/2001/XMLSchema-instance";
        internal const string schemaVt = @"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";

        internal const string schemaMainX14 = "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main";
        internal const string schemaMainX15 = "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main";
        internal const string schemaMainXm = "http://schemas.microsoft.com/office/excel/2006/main";
        internal const string schemaXr = "http://schemas.microsoft.com/office/spreadsheetml/2014/revision";
        internal const string schemaXr2 = "http://schemas.microsoft.com/office/spreadsheetml/2015/revision2";

        //Chart Ex
        internal const string schemaMc2006 = "http://schemas.openxmlformats.org/markup-compatibility/2006";
        internal const string schemaChartExMain = "http://schemas.microsoft.com/office/drawing/2014/chartex";
        internal const string schemaChartEx2015_9_8 = "http://schemas.microsoft.com/office/drawing/2015/9/8/chartex";
        internal const string schemaChartEx2015_10_21 = "http://schemas.microsoft.com/office/drawing/2015/10/21/chartex";
        internal const string schemaChartEx2016_5_10 = "http://schemas.microsoft.com/office/drawing/2016/5/10/chartex";
        internal const string schemaChartExRelationships = "http://schemas.microsoft.com/office/2014/relationships/chartEx";

        internal const string schemaSlicer = "http://schemas.microsoft.com/office/drawing/2012/slicer";
        internal const string schemaDrawings2010 = "http://schemas.microsoft.com/office/drawing/2010/main";
        internal const string schemaSlicer2010 = "http://schemas.microsoft.com/office/drawing/2010/slicer";
        internal const string schemaRelationshipsSlicer = "http://schemas.microsoft.com/office/2007/relationships/slicer";
        internal const string schemaRelationshipsSlicerCache = "http://schemas.microsoft.com/office/2007/relationships/slicerCache";
        //Threaded comments
        internal const string schemaThreadedComments = "http://schemas.microsoft.com/office/spreadsheetml/2018/threadedcomments";
        internal const string schemaThreadedComment = "http://schemas.microsoft.com/office/2017/10/relationships/threadedComment";
        //Persons
        internal const string schemaPersonsRelationShips = "http://schemas.microsoft.com/office/2017/10/relationships/person";

        // Richdata (used in worksheet.sortstate)
        internal const string schemaRichData2 = "http://schemas.microsoft.com/office/spreadsheetml/2017/richdata2";
    }
}
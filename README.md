# SpreadSheetConvertTool
Useful tool to make converter of GoogleSpreadSheet.


![spreadsheetconverttool_screenshot](https://github.com/charcolle/SpreadSheetConvertTool/blob/master/DescFiles/spreadSheetConvertTool_v1.0.png?raw=true)

### Supported Unity versions  
Unity 2017.1 or higher  
  
### All you need is
◆ClientID  
◆ClientSecret  
◆RefreshToken  
  
◆Your SpreadSheetId  
  
# Usage(In Progress)
## ◆Open Editor

1. Select [Window] ->[SpreadSheetConvertTool] .

## ◆Set GoogleSpreadSheet Config
  
1. You should get the data to access GoogleSheet in advance.  
1. Select [Config] tab at right colmn, then set your **ClientID**, **ClientSecret** and **RefreshToken**.  
1. If you press [Check Config] button, you can check if keys are correct.  
  
## ◆Create SpreadSheet Config
  
SpreadSheet manage each SheetConverters in this tool.  
  
1. Push *Green [+] Button*, name your SpreadSheetConfig data and push [Create] button.  
1. Set your **SpreadSheetId**.  

## ◆Create SheetConverter Script

1. Push *Green [+] Button*, name your converter script and push [Create] button.  
```csharp
comming soon...
```

## ◆Create SheetConverter Config

A SheetConverter has the system that convert GoogleSheet to your custom data and the settings which is the sheet name and the cell range and so on. Normally, a SheetConverter is for the one sheet. But you can convert multiple sheets if you have the sheets which has the same data structure.  

1. Push *[+] Button* to create your SheetConverter.  
1. Set your **SheetName**. You can get all of the sheet name from the SpreadSheet if you push [Refresh] button. You can also copy the sheet name to clipboard if you select the sheet name.
1. Set the cell range of the sheet.  
1 . You can check the response from GoogleSpreadSheet if you push [API Test] button.  

## ◆Create SheetConverter Config

1. Set the save path.  
1. Set the save filename and the file extension.  
NOTE: You dont need a dot with the file extension.
1. Push [Convert], then you can create your data!  

## ◆Use SheetConverter Script

comming soon...
```csharp
comming soon...
```

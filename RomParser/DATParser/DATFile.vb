﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.34014
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System.Xml.Serialization

'
'This source code was auto-generated by xsd, Version=4.0.30319.33440.
'

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440"),  _
 System.SerializableAttribute(),  _
 System.Diagnostics.DebuggerStepThroughAttribute(),  _
 System.ComponentModel.DesignerCategoryAttribute("code"),  _
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true),  _
 System.Xml.Serialization.XmlRootAttribute([Namespace]:="", IsNullable:=false)>  _
Partial Public Class datafile
    
    Private itemsField() As Object
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("game", GetType(datafileGame), Form:=System.Xml.Schema.XmlSchemaForm.Unqualified),  _
     System.Xml.Serialization.XmlElementAttribute("header", GetType(datafileHeader), Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property Items() As Object()
        Get
            Return Me.itemsField
        End Get
        Set
            Me.itemsField = value
        End Set
    End Property
End Class

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440"),  _
 System.SerializableAttribute(),  _
 System.Diagnostics.DebuggerStepThroughAttribute(),  _
 System.ComponentModel.DesignerCategoryAttribute("code"),  _
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true)>  _
Partial Public Class datafileGame
    
    Private descriptionField As String
    
    Private yearField As String
    
    Private manufacturerField As String
    
    Private romField() As datafileGameRom
    
    Private nameField As String
    
    Private cloneofField As String
    
    Private romofField As String
    
    Private ismechanicalField As String
    
    Private isbiosField As String
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property description() As String
        Get
            Return Me.descriptionField
        End Get
        Set
            Me.descriptionField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property year() As String
        Get
            Return Me.yearField
        End Get
        Set
            Me.yearField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property manufacturer() As String
        Get
            Return Me.manufacturerField
        End Get
        Set
            Me.manufacturerField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute("rom", Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property rom() As datafileGameRom()
        Get
            Return Me.romField
        End Get
        Set
            Me.romField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property name() As String
        Get
            Return Me.nameField
        End Get
        Set
            Me.nameField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property cloneof() As String
        Get
            Return Me.cloneofField
        End Get
        Set
            Me.cloneofField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property romof() As String
        Get
            Return Me.romofField
        End Get
        Set
            Me.romofField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property ismechanical() As String
        Get
            Return Me.ismechanicalField
        End Get
        Set
            Me.ismechanicalField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property isbios() As String
        Get
            Return Me.isbiosField
        End Get
        Set
            Me.isbiosField = value
        End Set
    End Property
End Class

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440"),  _
 System.SerializableAttribute(),  _
 System.Diagnostics.DebuggerStepThroughAttribute(),  _
 System.ComponentModel.DesignerCategoryAttribute("code"),  _
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true)>  _
Partial Public Class datafileGameRom
    
    Private nameField As String
    
    Private sizeField As String
    
    Private crcField As String

    Private md5Field As String
    
    Private sha1Field As String
    
    Private statusField As String
    
    Private mergeField As String
    
    Private biosField As String
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property name() As String
        Get
            Return Me.nameField
        End Get
        Set
            Me.nameField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property size() As String
        Get
            Return Me.sizeField
        End Get
        Set
            Me.sizeField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property crc() As String
        Get
            Return Me.crcField
        End Get
        Set
            Me.crcField = value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()> _
    Public Property md5() As String
        Get
            Return Me.md5Field
        End Get
        Set(value As String)
            Me.md5Field = value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property sha1() As String
        Get
            Return Me.sha1Field
        End Get
        Set
            Me.sha1Field = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property status() As String
        Get
            Return Me.statusField
        End Get
        Set
            Me.statusField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property merge() As String
        Get
            Return Me.mergeField
        End Get
        Set
            Me.mergeField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>  _
    Public Property bios() As String
        Get
            Return Me.biosField
        End Get
        Set
            Me.biosField = value
        End Set
    End Property
End Class

'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440"),  _
 System.SerializableAttribute(),  _
 System.Diagnostics.DebuggerStepThroughAttribute(),  _
 System.ComponentModel.DesignerCategoryAttribute("code"),  _
 System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=true)>  _
Partial Public Class datafileHeader
    
    Private nameField As String
    
    Private descriptionField As String
    
    Private categoryField As String
    
    Private versionField As String
    
    Private authorField As String
    
    Private commentField As String
    
    Private clrmameproField As String
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property name() As String
        Get
            Return Me.nameField
        End Get
        Set
            Me.nameField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property description() As String
        Get
            Return Me.descriptionField
        End Get
        Set
            Me.descriptionField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property category() As String
        Get
            Return Me.categoryField
        End Get
        Set
            Me.categoryField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property version() As String
        Get
            Return Me.versionField
        End Get
        Set
            Me.versionField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property author() As String
        Get
            Return Me.authorField
        End Get
        Set
            Me.authorField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property comment() As String
        Get
            Return Me.commentField
        End Get
        Set
            Me.commentField = value
        End Set
    End Property
    
    '''<remarks/>
    <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)>  _
    Public Property clrmamepro() As String
        Get
            Return Me.clrmameproField
        End Get
        Set
            Me.clrmameproField = value
        End Set
    End Property
End Class

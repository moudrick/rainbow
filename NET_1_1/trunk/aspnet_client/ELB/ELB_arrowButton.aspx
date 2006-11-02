<%@ Page Language="VB" ContentType="image/jpeg" Debug="true" %>
<%@ outputcache duration="3600" varybyparam="*" %>
<%@ import Namespace="ELB" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.Drawing.Imaging" %>
<script runat="server">

         ' This script is part of the EasyListBox server control.
         '  Purchase and licensing details can be found at http://EasyListBox.com .

         Public Sub Page_Load(ByVal Sender As Object, ByVal E As EventArgs)
             If Request.Querystring("ctlID") Is Nothing Then
                 ' Make the arrow button
                 Dim myELB As New EasyListBoxArrowButton()
                 ' myELB.WinXP = True ' Force XP-style buttons (don't forget to set your border to solid!)
                 Try
                     Dim bmpELB As Bitmap = myELB.CreateImage()
                     Dim MS as System.IO.MemoryStream = New System.IO.MemoryStream()
                     Response.Clear()
                     Response.ContentType = "Image/Png"
                     bmpELB.Save(MS,System.Drawing.Imaging.ImageFormat.Png)
                     Dim buffer as Byte() = MS.ToArray()
                     Response.OutputStream.Write(buffer,0,buffer.Length)
                     Response.End()

                 Catch ex As System.Exception
                     Response.ContentType = "text/html"
                     litMsg.Visible = True
                     litMsg.Text = "Error generating listbox arrow: <br />" & ex.ToString
                 End Try
             Else
                 Dim elbRequest As New EasyListBoxRequest()
                 Dim iChunkSize As Integer = System.Convert.ToInt32(Request.Querystring("chunkSize"))
                 Dim sHashCode As String = Request.Querystring("hashCode")
                 Dim sFilterValue As String = Request.Querystring("filterValue")
                 Dim sFilterType As String = Request.Querystring("filterType")
                 Response.ContentType = "text/html"

                 litMsg.Visible = True
                 litMsg.Text = elbRequest.GenerateItems(sFilterType, sFilterValue, sHashCode, iChunkSize)
             End If
         End Sub

</script>
<asp:Literal ID="litMsg" Visible="False" Runat="Server" />

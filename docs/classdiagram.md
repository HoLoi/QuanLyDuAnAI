<mxGraphModel dx="4031" dy="1994" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="850" pageHeight="1100" math="0" shadow="0">
  <root>
    <mxCell id="0" />
    <mxCell id="1" parent="0" />
    <mxCell id="xWTXjU9PBJvNTCzDRvnS-1" parent="1" style="swimlane;fontStyle=2;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=40;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="&lt;div&gt;{abstract}&lt;/div&gt;NguoiDung" vertex="1">
      <mxGeometry height="340" width="260" x="468" y="89" as="geometry">
        <mxRectangle height="30" width="100" x="400" y="100" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="xWTXjU9PBJvNTCzDRvnS-2" parent="xWTXjU9PBJvNTCzDRvnS-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaNguoiDung: int&lt;br&gt;+ HoTenNguoiDung: string&lt;br&gt;+ DiaChiNguoiDung: string&lt;br&gt;+ SdtNguoiDung: string&lt;br&gt;+ NgaySinh: DateTime&lt;br&gt;+ AnhDaiDien: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="250" width="260" y="40" as="geometry" />
    </mxCell>
    <mxCell id="xWTXjU9PBJvNTCzDRvnS-3" parent="xWTXjU9PBJvNTCzDRvnS-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="290" as="geometry" />
    </mxCell>
    <mxCell id="xWTXjU9PBJvNTCzDRvnS-4" parent="xWTXjU9PBJvNTCzDRvnS-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ capNhatThongTin(hoTen: string, email: string, soDienThoai: string): void" vertex="1">
      <mxGeometry height="42" width="260" y="298" as="geometry" />
    </mxCell>
    <mxCell id="mz6lyTV8cqTnrrgjEJ6o-1" edge="1" parent="1" source="TLakgcBGVkkkUZ1dkReR-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;entryX=1;entryY=0.856;entryDx=0;entryDy=0;entryPerimeter=0;" target="5xCHvEqshszrvCJ6u1h0-2">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="2040" y="1880" />
          <mxPoint x="190" y="1880" />
        </Array>
        <mxPoint x="1460" y="620" as="sourcePoint" />
        <mxPoint x="280" y="1848" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-117" connectable="0" parent="mz6lyTV8cqTnrrgjEJ6o-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9763" y="3" as="geometry">
        <mxPoint x="17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-118" connectable="0" parent="mz6lyTV8cqTnrrgjEJ6o-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;◀ duyệt&amp;nbsp;&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.5464" y="3" as="geometry">
        <mxPoint x="17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-119" connectable="0" parent="mz6lyTV8cqTnrrgjEJ6o-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9246" y="-3" as="geometry">
        <mxPoint x="14" y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="93-mJI576MT82vWcGMnV-4" edge="1" parent="1" source="6wtPaquReI3S4elhgb0T-1" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="nGZGdnT23SMgTuQgHzuC-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1720" y="-200" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-150" connectable="0" parent="93-mJI576MT82vWcGMnV-4" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9692" y="2" as="geometry">
        <mxPoint x="8" y="-12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-151" connectable="0" parent="93-mJI576MT82vWcGMnV-4" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.963" as="geometry">
        <mxPoint x="20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-152" connectable="0" parent="93-mJI576MT82vWcGMnV-4" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ duyệt&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.5379" y="3" as="geometry">
        <mxPoint x="17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="6wtPaquReI3S4elhgb0T-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="QuanTriVien" vertex="1">
      <mxGeometry height="136" width="260" x="704" y="-240" as="geometry">
        <mxRectangle height="30" width="110" x="-460" y="80" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="6wtPaquReI3S4elhgb0T-2" parent="6wtPaquReI3S4elhgb0T-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="4" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="6wtPaquReI3S4elhgb0T-3" parent="6wtPaquReI3S4elhgb0T-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="30" as="geometry" />
    </mxCell>
    <mxCell id="6wtPaquReI3S4elhgb0T-4" parent="6wtPaquReI3S4elhgb0T-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoNguoiDung(nguoiDung: NguoiDung): void&lt;br&gt;+ khoaNguoiDung(maNguoiDung: int): void&lt;br&gt;+ quanLyVaiTro(vaiTro: VaiTro): void&lt;br&gt;+ capQuyen(maVaiTroHeThong: int, maChucNang: int, coQuyen: bool): void&lt;br&gt;+ quanLyNhanSu(nhanVien: NhanVien): void" vertex="1">
      <mxGeometry height="98" width="260" y="38" as="geometry" />
    </mxCell>
    <mxCell id="6wtPaquReI3S4elhgb0T-5" edge="1" parent="1" source="6wtPaquReI3S4elhgb0T-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=block;endFill=0;endSize=15;" target="xWTXjU9PBJvNTCzDRvnS-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="enBTkT7pxzC_SMcUWDWl-6" edge="1" parent="1" source="TLakgcBGVkkkUZ1dkReR-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1980" y="1520" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-17" connectable="0" parent="enBTkT7pxzC_SMcUWDWl-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9647" y="2" as="geometry">
        <mxPoint x="9" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-18" connectable="0" parent="enBTkT7pxzC_SMcUWDWl-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=90;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;Quản lý ▶&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.1028" y="1" as="geometry">
        <mxPoint x="19" y="-4" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-19" connectable="0" parent="enBTkT7pxzC_SMcUWDWl-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9743" y="-4" as="geometry">
        <mxPoint x="3" y="-11" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="5grzo2p5ejAfA8UIloDJ-5" edge="1" parent="1" source="TLakgcBGVkkkUZ1dkReR-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="5grzo2p5ejAfA8UIloDJ-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1880" y="600" />
          <mxPoint x="-1160" y="600" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="kW6yUhoyHavPNyB8MbnI-21" connectable="0" parent="5grzo2p5ejAfA8UIloDJ-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="◀&amp;nbsp; dánh giá" vertex="1">
      <mxGeometry relative="1" x="0.1068" y="2" as="geometry">
        <mxPoint x="1" y="-19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-153" connectable="0" parent="5grzo2p5ejAfA8UIloDJ-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.977" as="geometry">
        <mxPoint x="10" y="2" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-154" connectable="0" parent="5grzo2p5ejAfA8UIloDJ-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9832" y="1" as="geometry">
        <mxPoint x="19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="nGZGdnT23SMgTuQgHzuC-5" edge="1" parent="1" source="TLakgcBGVkkkUZ1dkReR-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="nGZGdnT23SMgTuQgHzuC-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1920" y="660" />
          <mxPoint x="1920" y="660" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="kW6yUhoyHavPNyB8MbnI-32" connectable="0" parent="nGZGdnT23SMgTuQgHzuC-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=90;fontSize=20;" value="Đánh giá&amp;nbsp; ▶" vertex="1">
      <mxGeometry relative="1" x="0.0196" y="1" as="geometry">
        <mxPoint x="19" y="4" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-142" connectable="0" parent="nGZGdnT23SMgTuQgHzuC-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9234" y="3" as="geometry">
        <mxPoint x="7" y="1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-143" connectable="0" parent="nGZGdnT23SMgTuQgHzuC-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9513" y="-5" as="geometry">
        <mxPoint x="1" y="-24" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="93-mJI576MT82vWcGMnV-1" edge="1" parent="1" source="TLakgcBGVkkkUZ1dkReR-1" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="autogen-30">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="2520" y="320" />
          <mxPoint x="2520" y="3080" />
          <mxPoint x="-760" y="3080" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-127" connectable="0" parent="93-mJI576MT82vWcGMnV-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;◀ duyệt&amp;nbsp;&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.5015" y="3" as="geometry">
        <mxPoint y="17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-131" connectable="0" parent="93-mJI576MT82vWcGMnV-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9882" y="4" as="geometry">
        <mxPoint y="-16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-133" connectable="0" parent="93-mJI576MT82vWcGMnV-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9905" y="-1" as="geometry">
        <mxPoint x="19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="93-mJI576MT82vWcGMnV-2" edge="1" parent="1" source="TLakgcBGVkkkUZ1dkReR-1" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="autogen-34">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="2480" y="360" />
          <mxPoint x="2480" y="3040" />
          <mxPoint x="-560" y="3040" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-126" connectable="0" parent="93-mJI576MT82vWcGMnV-2" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9918" y="-3" as="geometry">
        <mxPoint x="17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-130" connectable="0" parent="93-mJI576MT82vWcGMnV-2" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ duyệt&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.4447" as="geometry">
        <mxPoint x="-7" y="20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-132" connectable="0" parent="93-mJI576MT82vWcGMnV-2" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.988" y="2" as="geometry">
        <mxPoint y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="TLakgcBGVkkkUZ1dkReR-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="QuanLy" vertex="1">
      <mxGeometry height="190" width="260" x="1820" y="220" as="geometry">
        <mxRectangle height="30" width="80" x="770" y="370" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="TLakgcBGVkkkUZ1dkReR-2" parent="TLakgcBGVkkkUZ1dkReR-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="4" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="TLakgcBGVkkkUZ1dkReR-3" parent="TLakgcBGVkkkUZ1dkReR-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="30" as="geometry" />
    </mxCell>
    <mxCell id="TLakgcBGVkkkUZ1dkReR-4" parent="TLakgcBGVkkkUZ1dkReR-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoDuAn(duAn: DuAn): void&lt;br&gt;+ capNhatDuAn(duAn: DuAn): void&lt;br&gt;+ duyetDeXuatCongViec(maCongViec: int, chapNhan: bool): void&lt;br&gt;+ duyetDeXuatNganSach(maChiPhi: int, chapNhan: bool): void&lt;br&gt;+ xemBaoCao(maDuAn: int): string&lt;br&gt;+ danhGiaDuAn(maDuAn: int): void" vertex="1">
      <mxGeometry height="152" width="260" y="38" as="geometry" />
    </mxCell>
    <mxCell id="TLakgcBGVkkkUZ1dkReR-5" edge="1" parent="1" source="TLakgcBGVkkkUZ1dkReR-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=block;endFill=0;endSize=15;edgeStyle=orthogonalEdgeStyle;" target="xWTXjU9PBJvNTCzDRvnS-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1290" y="240" />
          <mxPoint x="1290" y="240" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="3Mr1UYKB6cySFUXDPDW_-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="AspNetRoles" vertex="1">
      <mxGeometry height="170" width="260" x="-821.91" y="-165" as="geometry">
        <mxRectangle height="30" width="70" x="140" y="100" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="3Mr1UYKB6cySFUXDPDW_-2" parent="3Mr1UYKB6cySFUXDPDW_-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ Id: string&lt;br&gt;+ Name: string&lt;br&gt;+ NormalizedName: string&lt;br&gt;+ ConcurrencyStamp: string" vertex="1">
      <mxGeometry height="78" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="3Mr1UYKB6cySFUXDPDW_-3" parent="3Mr1UYKB6cySFUXDPDW_-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="104" as="geometry" />
    </mxCell>
    <mxCell id="3Mr1UYKB6cySFUXDPDW_-4" parent="3Mr1UYKB6cySFUXDPDW_-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoVaiTroHeThong(tenVaiTro: string): void&lt;br&gt;+ capNhatVaiTroHeThong(tenVaiTro: string): void" vertex="1">
      <mxGeometry height="58" width="260" y="112" as="geometry" />
    </mxCell>
    <mxCell id="O05rKdK9spuyOT-CUAnu-5" edge="1" parent="1" source="O05rKdK9spuyOT-CUAnu-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=block;endFill=0;startSize=6;endSize=15;edgeStyle=orthogonalEdgeStyle;" target="xWTXjU9PBJvNTCzDRvnS-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-760" y="320" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="LcXY-409vzfgchgwM_3A-6" edge="1" parent="1" source="O05rKdK9spuyOT-CUAnu-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="LcXY-409vzfgchgwM_3A-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-1680" y="1494" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="09SOyR6Pm2KEEurYv0xO-3" connectable="0" parent="LcXY-409vzfgchgwM_3A-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9386" y="-3" as="geometry">
        <mxPoint x="1" y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="09SOyR6Pm2KEEurYv0xO-4" connectable="0" parent="LcXY-409vzfgchgwM_3A-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9593" y="2" as="geometry">
        <mxPoint x="22" y="-26" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="09SOyR6Pm2KEEurYv0xO-5" connectable="0" parent="LcXY-409vzfgchgwM_3A-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;◀ cập nhật&amp;nbsp;&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0831" y="-1" as="geometry">
        <mxPoint x="14" y="-23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="Jqhau5jHDxXsKYdf8Zkh-5" edge="1" parent="1" source="O05rKdK9spuyOT-CUAnu-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="Jqhau5jHDxXsKYdf8Zkh-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-720" y="730" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="kW6yUhoyHavPNyB8MbnI-24" connectable="0" parent="Jqhau5jHDxXsKYdf8Zkh-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=0;fontSize=20;" value="có&amp;nbsp; ▶" vertex="1">
      <mxGeometry relative="1" x="-0.0651" y="2" as="geometry">
        <mxPoint x="-1" y="-14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-6" connectable="0" parent="Jqhau5jHDxXsKYdf8Zkh-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.977" y="-1" as="geometry">
        <mxPoint x="12" y="-1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-7" connectable="0" parent="Jqhau5jHDxXsKYdf8Zkh-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9633" y="3" as="geometry">
        <mxPoint y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-5" edge="1" parent="1" source="O05rKdK9spuyOT-CUAnu-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="FGaaYxJouGr9IegEVFHG-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-1440" y="1540" />
        </Array>
        <mxPoint x="-797.7259633027518" y="968" as="sourcePoint" />
        <mxPoint x="-810.0011926605505" y="1860" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="09SOyR6Pm2KEEurYv0xO-1" connectable="0" parent="G1Bnz5JsvCSpgI5tl59X-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9235" y="-2" as="geometry">
        <mxPoint x="1" y="-16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="09SOyR6Pm2KEEurYv0xO-2" connectable="0" parent="G1Bnz5JsvCSpgI5tl59X-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9158" y="1" as="geometry">
        <mxPoint x="19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-11" edge="1" parent="1" source="O05rKdK9spuyOT-CUAnu-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="G1Bnz5JsvCSpgI5tl59X-7">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="09SOyR6Pm2KEEurYv0xO-6" connectable="0" parent="G1Bnz5JsvCSpgI5tl59X-11" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.816" y="-1" as="geometry">
        <mxPoint y="-23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="09SOyR6Pm2KEEurYv0xO-7" connectable="0" parent="G1Bnz5JsvCSpgI5tl59X-11" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.907" y="-2" as="geometry">
        <mxPoint x="13" y="-14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="09SOyR6Pm2KEEurYv0xO-8" connectable="0" parent="G1Bnz5JsvCSpgI5tl59X-11" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=30;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;◀ có&amp;nbsp;&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.1133" y="-5" as="geometry">
        <mxPoint x="15" y="-5" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="yhDT0zH1PYg4IR5jnH_h-5" edge="1" parent="1" source="O05rKdK9spuyOT-CUAnu-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="yhDT0zH1PYg4IR5jnH_h-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-640" y="1060" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-8" connectable="0" parent="yhDT0zH1PYg4IR5jnH_h-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9461" y="-2" as="geometry">
        <mxPoint x="23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-9" connectable="0" parent="yhDT0zH1PYg4IR5jnH_h-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9317" y="2" as="geometry">
        <mxPoint x="1" y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="Jpoa3KNm6k-TmzMDOyCY-1" edge="1" parent="1" source="O05rKdK9spuyOT-CUAnu-1" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-680" y="860" />
          <mxPoint x="800" y="860" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="09SOyR6Pm2KEEurYv0xO-9" connectable="0" parent="Jpoa3KNm6k-TmzMDOyCY-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9715" y="-2" as="geometry">
        <mxPoint x="14" y="9" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="09SOyR6Pm2KEEurYv0xO-10" connectable="0" parent="Jpoa3KNm6k-TmzMDOyCY-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9731" y="6" as="geometry">
        <mxPoint x="12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="O05rKdK9spuyOT-CUAnu-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NhanVien" vertex="1">
      <mxGeometry height="190" width="260" x="-890" y="1410" as="geometry">
        <mxRectangle height="30" width="90" x="740" y="920" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="O05rKdK9spuyOT-CUAnu-3" parent="O05rKdK9spuyOT-CUAnu-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="O05rKdK9spuyOT-CUAnu-4" parent="O05rKdK9spuyOT-CUAnu-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ capNhatTienDo(maCongViec: int, phanTram: int, ghiChu: string): void&lt;br&gt;+ deXuatCongViec(congViec: CongViec): void&lt;br&gt;+ deXuatNganSach(chiPhi: NganSach): void&lt;br&gt;+ ghiNhatKyPhanCong(maCongViec: int, noiDung: string): void&lt;br&gt;+ guiTinNhan(maPhong: int, noiDung: string): void&lt;div&gt;+ phanCongCongViec(maCongViec: int, maNhanVien: int): void&lt;br&gt;&lt;/div&gt;" vertex="1">
      <mxGeometry height="156" width="260" y="34" as="geometry" />
    </mxCell>
    <mxCell id="qPm_Ca8Bxiu_PFRpAWqy-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="AspNetRoleClaims" vertex="1">
      <mxGeometry height="132" width="150" x="-1295.22" y="-152" as="geometry">
        <mxRectangle height="30" width="100" x="-275" y="550" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="qPm_Ca8Bxiu_PFRpAWqy-2" parent="qPm_Ca8Bxiu_PFRpAWqy-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ Id: int&lt;br&gt;+ ClaimType: string&lt;br&gt;+ ClaimValue: string" vertex="1">
      <mxGeometry height="54" width="150" y="26" as="geometry" />
    </mxCell>
    <mxCell id="qPm_Ca8Bxiu_PFRpAWqy-3" parent="qPm_Ca8Bxiu_PFRpAWqy-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="150" y="80" as="geometry" />
    </mxCell>
    <mxCell id="qPm_Ca8Bxiu_PFRpAWqy-4" parent="qPm_Ca8Bxiu_PFRpAWqy-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ capQuyen(): void&lt;br/&gt;+ thuHoiQuyen(): void" vertex="1">
      <mxGeometry height="44" width="150" y="88" as="geometry" />
    </mxCell>
    <mxCell id="qPm_Ca8Bxiu_PFRpAWqy-5" edge="1" parent="1" source="qPm_Ca8Bxiu_PFRpAWqy-2" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;entryX=0;entryY=0.5;entryDx=0;entryDy=0;endArrow=none;endFill=0;endSize=15;" target="3Mr1UYKB6cySFUXDPDW_-2">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="YG-KSfH8bEd0ezlJah6r-2" connectable="0" parent="qPm_Ca8Bxiu_PFRpAWqy-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="1" vertex="1">
      <mxGeometry relative="1" x="-0.0589" y="1" as="geometry">
        <mxPoint x="143" y="-13" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="YG-KSfH8bEd0ezlJah6r-3" connectable="0" parent="qPm_Ca8Bxiu_PFRpAWqy-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="0..*" vertex="1">
      <mxGeometry relative="1" x="0.0428" y="1" as="geometry">
        <mxPoint x="-134" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="DHByf7-s3Gl3JD-fI9mn-11" connectable="0" parent="qPm_Ca8Bxiu_PFRpAWqy-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="◀&amp;nbsp; có" vertex="1">
      <mxGeometry relative="1" x="-0.0858" y="1" as="geometry">
        <mxPoint x="6" y="-12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="qPm_Ca8Bxiu_PFRpAWqy-11" edge="1" parent="1" source="qPm_Ca8Bxiu_PFRpAWqy-6" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;dashed=1;">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="-401.91" y="-82" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="qPm_Ca8Bxiu_PFRpAWqy-6" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="AspNetUserRoles" vertex="1">
      <mxGeometry height="34" width="160" x="-481.91" y="-199" as="geometry">
        <mxRectangle height="30" width="70" x="140" y="100" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="qPm_Ca8Bxiu_PFRpAWqy-8" parent="qPm_Ca8Bxiu_PFRpAWqy-6" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="160" y="26" as="geometry" />
    </mxCell>
    <mxCell id="qPm_Ca8Bxiu_PFRpAWqy-10" edge="1" parent="1" source="3Mr1UYKB6cySFUXDPDW_-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;flowAnimation=0;" target="GlT-6QYPskowLD9cseYI-1">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="-71.91" y="20" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="A_Su_KYVSErlhegYWDCW-8" connectable="0" parent="qPm_Ca8Bxiu_PFRpAWqy-10" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7405" as="geometry">
        <mxPoint x="-17" y="-26" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="A_Su_KYVSErlhegYWDCW-9" connectable="0" parent="qPm_Ca8Bxiu_PFRpAWqy-10" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7437" y="2" as="geometry">
        <mxPoint y="-24" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="GlT-6QYPskowLD9cseYI-1" parent="1" style="swimlane;fontStyle=2;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=25;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="&lt;div&gt;AspNetUsers&lt;/div&gt;" vertex="1">
      <mxGeometry height="400" width="280" x="-203.91" y="-282" as="geometry">
        <mxRectangle height="30" width="100" x="400" y="100" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="GlT-6QYPskowLD9cseYI-2" parent="GlT-6QYPskowLD9cseYI-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ Id: string&lt;br&gt;+ UserName: string&lt;br&gt;+ NormalizedUserName: string&lt;br&gt;+ Email: string&lt;br&gt;+ NormalizedEmail: string&lt;br&gt;+ EmailConfirmed: bool&lt;br&gt;+ PasswordHash: string&lt;br&gt;+ SecurityStamp: string&lt;br&gt;+ ConcurrencyStamp: string&lt;br&gt;+ PhoneNumber: string&lt;br&gt;+ PhoneNumberConfirmed: bool&lt;br&gt;+ TwoFactorEnabled: bool&lt;br&gt;+ LockoutEnd: DateTime&lt;br&gt;+ LockoutEnabled: bool&lt;br&gt;+ AccessFailedCount: int" vertex="1">
      <mxGeometry height="245" width="280" y="25" as="geometry" />
    </mxCell>
    <mxCell id="GlT-6QYPskowLD9cseYI-3" parent="GlT-6QYPskowLD9cseYI-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="280" y="270" as="geometry" />
    </mxCell>
    <mxCell id="GlT-6QYPskowLD9cseYI-4" parent="GlT-6QYPskowLD9cseYI-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ dangNhap(tenDangNhap: string, matKhau: string): boolean&lt;br&gt;+ dangXuat(): void&lt;br&gt;+ doiMatKhau(matKhauCu: string, matKhauMoi: string): bool&lt;br&gt;+ capNhatThongTin(hoTen: string, email: string, soDienThoai: string): void" vertex="1">
      <mxGeometry height="122" width="280" y="278" as="geometry" />
    </mxCell>
    <mxCell id="GlT-6QYPskowLD9cseYI-5" edge="1" parent="1" source="GlT-6QYPskowLD9cseYI-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="xWTXjU9PBJvNTCzDRvnS-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="520" y="40" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="GlT-6QYPskowLD9cseYI-8" connectable="0" parent="GlT-6QYPskowLD9cseYI-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="◀&amp;nbsp; thuộc" vertex="1">
      <mxGeometry relative="1" x="0.0246" as="geometry">
        <mxPoint y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ZYtPWBJm8RI_xscLBX3h-3" connectable="0" parent="GlT-6QYPskowLD9cseYI-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;1&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8892" y="2" as="geometry">
        <mxPoint y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ZYtPWBJm8RI_xscLBX3h-4" connectable="0" parent="GlT-6QYPskowLD9cseYI-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;1&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.888" as="geometry">
        <mxPoint x="10" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="AoST3ZVm1FiiXK3rUbv--1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="DanhMucManHinh" vertex="1">
      <mxGeometry height="102" width="140" x="-1777.5" y="-426" as="geometry">
        <mxRectangle height="30" width="140" x="-295" y="790" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="AoST3ZVm1FiiXK3rUbv--2" parent="AoST3ZVm1FiiXK3rUbv--1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaManHinh: int&lt;br&gt;+ TenManHinh: string&lt;br&gt;+ MoTaManHinh: string" vertex="1">
      <mxGeometry height="60" width="140" y="26" as="geometry" />
    </mxCell>
    <mxCell id="AoST3ZVm1FiiXK3rUbv--3" parent="AoST3ZVm1FiiXK3rUbv--1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="140" y="86" as="geometry" />
    </mxCell>
    <mxCell id="AoST3ZVm1FiiXK3rUbv--4" parent="AoST3ZVm1FiiXK3rUbv--1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="8" width="140" y="94" as="geometry" />
    </mxCell>
    <mxCell id="YG-KSfH8bEd0ezlJah6r-1" edge="1" parent="1" source="AoST3ZVm1FiiXK3rUbv--1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;startArrow=diamondThin;startFill=1;startSize=15;" target="DHByf7-s3Gl3JD-fI9mn-1">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="-1645" y="22" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="DHByf7-s3Gl3JD-fI9mn-13" connectable="0" parent="YG-KSfH8bEd0ezlJah6r-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;fontSize=20;" value="thuộc&amp;nbsp; ▶" vertex="1">
      <mxGeometry relative="1" x="-0.0209" y="-2" as="geometry">
        <mxPoint x="16" y="-2" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="DHByf7-s3Gl3JD-fI9mn-5" edge="1" parent="1" source="DHByf7-s3Gl3JD-fI9mn-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="qPm_Ca8Bxiu_PFRpAWqy-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="DHByf7-s3Gl3JD-fI9mn-6" connectable="0" parent="DHByf7-s3Gl3JD-fI9mn-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="1" vertex="1">
      <mxGeometry relative="1" x="-0.0039" y="1" as="geometry">
        <mxPoint x="-149" y="-14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="DHByf7-s3Gl3JD-fI9mn-10" connectable="0" parent="DHByf7-s3Gl3JD-fI9mn-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="0..*" vertex="1">
      <mxGeometry relative="1" x="0.1022" y="-2" as="geometry">
        <mxPoint x="123" y="-20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="DHByf7-s3Gl3JD-fI9mn-12" connectable="0" parent="DHByf7-s3Gl3JD-fI9mn-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="◀&amp;nbsp; thuộc" vertex="1">
      <mxGeometry relative="1" x="-0.1176" y="-1" as="geometry">
        <mxPoint x="5" y="-16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="DHByf7-s3Gl3JD-fI9mn-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="DanhMucQuyen" vertex="1">
      <mxGeometry height="98" width="185" x="-1800" y="-135" as="geometry">
        <mxRectangle height="30" width="150" x="-1260" y="290" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="DHByf7-s3Gl3JD-fI9mn-2" parent="DHByf7-s3Gl3JD-fI9mn-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaDanhMucQuyen: int&lt;br&gt;+ TenDanhMucQuyen: string&lt;br&gt;+ MoTaDanhMucQuyen: string" vertex="1">
      <mxGeometry height="64" width="185" y="26" as="geometry" />
    </mxCell>
    <mxCell id="DHByf7-s3Gl3JD-fI9mn-3" parent="DHByf7-s3Gl3JD-fI9mn-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="185" y="90" as="geometry" />
    </mxCell>
    <mxCell id="MAO-DfN84yR15LXXRfSc-5" edge="1" parent="1" source="enBTkT7pxzC_SMcUWDWl-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="MAO-DfN84yR15LXXRfSc-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="919" y="2240" />
          <mxPoint x="919" y="2240" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-13" connectable="0" parent="MAO-DfN84yR15LXXRfSc-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9203" y="1" as="geometry">
        <mxPoint x="29" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-14" connectable="0" parent="MAO-DfN84yR15LXXRfSc-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9213" y="4" as="geometry">
        <mxPoint x="14" y="1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-15" connectable="0" parent="MAO-DfN84yR15LXXRfSc-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.024" as="geometry">
        <mxPoint x="21" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="YC7cCPmTWb5SRYQHqm1z-5" edge="1" parent="1" source="enBTkT7pxzC_SMcUWDWl-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="YC7cCPmTWb5SRYQHqm1z-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-22" connectable="0" parent="YC7cCPmTWb5SRYQHqm1z-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8031" y="4" as="geometry">
        <mxPoint x="17" y="8" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-23" connectable="0" parent="YC7cCPmTWb5SRYQHqm1z-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7816" y="6" as="geometry">
        <mxPoint x="13" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-24" connectable="0" parent="YC7cCPmTWb5SRYQHqm1z-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-60;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;◀ thuộc&amp;nbsp;&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0296" y="5" as="geometry">
        <mxPoint y="32" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="LE_D31fqEWLUAxVKmXNx-5" edge="1" parent="1" source="enBTkT7pxzC_SMcUWDWl-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="LE_D31fqEWLUAxVKmXNx-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="enBTkT7pxzC_SMcUWDWl-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="DuAn" vertex="1">
      <mxGeometry height="392" width="200" x="750" y="1360" as="geometry">
        <mxRectangle height="30" width="70" x="820" y="1110" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="enBTkT7pxzC_SMcUWDWl-2" parent="enBTkT7pxzC_SMcUWDWl-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaDuAn: int&lt;br&gt;+ TenDuAn: string&lt;br&gt;+ MoTaDuAn: string&lt;br&gt;+ NgayTaoDuAn: DateTime&lt;br&gt;+ NgayBatDauDuAn: DateTime&lt;br&gt;+ NgayKetThucDuAn: DateTime&lt;br&gt;+ PhanTramHoanThanh: int&lt;br&gt;+ TrangThaiDuAn: string&lt;br&gt;+ GhiChuDuAn: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="224" width="200" y="26" as="geometry" />
    </mxCell>
    <mxCell id="enBTkT7pxzC_SMcUWDWl-3" parent="enBTkT7pxzC_SMcUWDWl-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="200" y="250" as="geometry" />
    </mxCell>
    <mxCell id="enBTkT7pxzC_SMcUWDWl-4" parent="enBTkT7pxzC_SMcUWDWl-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoDuAn(): void&lt;br&gt;+ capNhatDuAn(): void&lt;br&gt;+ xoaDuAn(): void&lt;br&gt;+ xemChiTietDuAn(): void&lt;br&gt;+ xacNhanHoanThanh(): void&lt;br&gt;+ tamDungDuAn(): void&lt;br&gt;+ moLaiDuAn(): void" vertex="1">
      <mxGeometry height="134" width="200" y="258" as="geometry" />
    </mxCell>
    <mxCell id="FGaaYxJouGr9IegEVFHG-5" edge="1" parent="1" source="MAO-DfN84yR15LXXRfSc-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="FGaaYxJouGr9IegEVFHG-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="919" y="2980" />
          <mxPoint x="-1505" y="2980" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-56" connectable="0" parent="FGaaYxJouGr9IegEVFHG-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9838" y="4" as="geometry">
        <mxPoint x="12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-57" connectable="0" parent="FGaaYxJouGr9IegEVFHG-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9785" as="geometry">
        <mxPoint x="25" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-58" connectable="0" parent="FGaaYxJouGr9IegEVFHG-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Thuộc ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.2127" y="-3" as="geometry">
        <mxPoint y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="MAO-DfN84yR15LXXRfSc-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="DanhMucCongViec" vertex="1">
      <mxGeometry height="184" width="170" x="834" y="2708" as="geometry">
        <mxRectangle height="30" width="140" x="480" y="840" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="MAO-DfN84yR15LXXRfSc-2" parent="MAO-DfN84yR15LXXRfSc-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaDanhMucCV: int&lt;br&gt;+ TenDanhMucCV: string&lt;br&gt;+ MoTaDanhMucCV: string&lt;br&gt;+ NgayTaoDMCV: DateTime&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="124" width="170" y="26" as="geometry" />
    </mxCell>
    <mxCell id="MAO-DfN84yR15LXXRfSc-3" parent="MAO-DfN84yR15LXXRfSc-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="170" y="150" as="geometry" />
    </mxCell>
    <mxCell id="MAO-DfN84yR15LXXRfSc-4" parent="MAO-DfN84yR15LXXRfSc-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoDanhMuc(): void" vertex="1">
      <mxGeometry height="26" width="170" y="158" as="geometry" />
    </mxCell>
    <mxCell id="sSkXnrdGMHA7VeClHVL3-5" edge="1" parent="1" source="FGaaYxJouGr9IegEVFHG-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="sSkXnrdGMHA7VeClHVL3-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="H2kTQfWC1UXGSywkNDVl-65" connectable="0" parent="sSkXnrdGMHA7VeClHVL3-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-35;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0087" y="1" as="geometry">
        <mxPoint x="-18" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="NvCRhkCUS0AH-Iu4hbs--6" edge="1" parent="1" source="FGaaYxJouGr9IegEVFHG-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="NvCRhkCUS0AH-Iu4hbs--1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="FGaaYxJouGr9IegEVFHG-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="CongViec" vertex="1">
      <mxGeometry height="390" width="230" x="-1620" y="1713" as="geometry">
        <mxRectangle height="30" width="90" x="120" y="950" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="FGaaYxJouGr9IegEVFHG-2" parent="FGaaYxJouGr9IegEVFHG-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaCongViec: int&lt;br&gt;+ TenCongViec: string&lt;br&gt;+ MoTaCongViec: string&lt;br&gt;+ NgayBatDauCongViec: DateTime&lt;br&gt;+ NgayKetThucCVDuKien: DateTime&lt;br&gt;+ NgayKetThucCVThucTe: DateTime&lt;br&gt;+ NgayTaoCongViec: DateTime&lt;br&gt;+ TrangThaiCongViec: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="258" width="230" y="26" as="geometry" />
    </mxCell>
    <mxCell id="FGaaYxJouGr9IegEVFHG-3" parent="FGaaYxJouGr9IegEVFHG-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="230" y="284" as="geometry" />
    </mxCell>
    <mxCell id="FGaaYxJouGr9IegEVFHG-4" parent="FGaaYxJouGr9IegEVFHG-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoCongViec(): void&lt;br&gt;+ capNhatCongViec(): void&lt;br&gt;+ xoaCongViec(): void&lt;br&gt;+ xacNhanHoanThanhCongViec(): void&lt;br&gt;+ moLaiCongViec(): void" vertex="1">
      <mxGeometry height="98" width="230" y="292" as="geometry" />
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-1" edge="1" parent="1" source="sSkXnrdGMHA7VeClHVL3-1" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="O05rKdK9spuyOT-CUAnu-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-800" y="2238" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-21" connectable="0" parent="ob-vmPQV9zG-7l0FT_MR-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.963" y="-3" as="geometry">
        <mxPoint x="15" y="4" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-22" connectable="0" parent="ob-vmPQV9zG-7l0FT_MR-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9552" y="2" as="geometry">
        <mxPoint x="-4" y="-16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="sSkXnrdGMHA7VeClHVL3-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="CtCongViec" vertex="1">
      <mxGeometry height="300" width="190" x="-2300" y="2088" as="geometry">
        <mxRectangle height="30" width="100" x="-420" y="950" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="sSkXnrdGMHA7VeClHVL3-2" parent="sSkXnrdGMHA7VeClHVL3-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaChiTietCV: int&lt;br&gt;+ TenCTCV: string&lt;br&gt;+ NoiDungChiTietCV: string&lt;br&gt;+ NgayTaoCTCV: DateTime&lt;br&gt;+ NgayBatDauCTCV: DateTime&lt;br&gt;+ NgayKetThucCTCV: DateTime&lt;br&gt;+ TrangThaiCTCV: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="204" width="190" y="26" as="geometry" />
    </mxCell>
    <mxCell id="sSkXnrdGMHA7VeClHVL3-3" parent="sSkXnrdGMHA7VeClHVL3-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="190" y="230" as="geometry" />
    </mxCell>
    <mxCell id="sSkXnrdGMHA7VeClHVL3-4" parent="sSkXnrdGMHA7VeClHVL3-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ themChiTiet(): void&lt;br&gt;+ capNhatChiTiet(): void&lt;br&gt;+ xoaChiTiet(): void" vertex="1">
      <mxGeometry height="62" width="190" y="238" as="geometry" />
    </mxCell>
    <mxCell id="YC7cCPmTWb5SRYQHqm1z-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="LoaiDuAn" vertex="1">
      <mxGeometry height="102" width="160" x="490" y="2050" as="geometry">
        <mxRectangle height="30" width="90" x="990" y="860" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="YC7cCPmTWb5SRYQHqm1z-2" parent="YC7cCPmTWb5SRYQHqm1z-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaLoaiDuAn: int&lt;br&gt;+ TenLoai: string&lt;br&gt;+ MoTaLoaiDuAn: string" vertex="1">
      <mxGeometry height="60" width="160" y="26" as="geometry" />
    </mxCell>
    <mxCell id="YC7cCPmTWb5SRYQHqm1z-3" parent="YC7cCPmTWb5SRYQHqm1z-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="160" y="86" as="geometry" />
    </mxCell>
    <mxCell id="YC7cCPmTWb5SRYQHqm1z-4" parent="YC7cCPmTWb5SRYQHqm1z-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="8" width="160" y="94" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-68" edge="1" parent="1" source="LcXY-409vzfgchgwM_3A-1" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="TLakgcBGVkkkUZ1dkReR-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-1750" y="560" />
          <mxPoint x="1840" y="560" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-155" connectable="0" parent="G3jGO_3h6dEc7QatDs8r-68" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ duyệt&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0011" y="4" as="geometry">
        <mxPoint y="-16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-156" connectable="0" parent="G3jGO_3h6dEc7QatDs8r-68" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.982" y="-1" as="geometry">
        <mxPoint x="9" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-157" connectable="0" parent="G3jGO_3h6dEc7QatDs8r-68" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9865" y="1" as="geometry">
        <mxPoint x="21" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="LcXY-409vzfgchgwM_3A-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="TienDoCongViec" vertex="1">
      <mxGeometry height="254" width="200" x="-1850" y="1366" as="geometry">
        <mxRectangle height="30" width="130" x="-350" y="700" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="LcXY-409vzfgchgwM_3A-2" parent="LcXY-409vzfgchgwM_3A-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaTienDo: int&lt;br&gt;+ ThoiGianDuyet: DateTime&lt;br&gt;+ GhiChuDuyet: string&lt;br&gt;+ PhanTram: int&lt;br&gt;+ GhiChuTienDo: string&lt;br&gt;+ ThoiGianCapNhat: DateTime&lt;br&gt;+ TrangThaiCTCVDeXuat: string&lt;br&gt;+ TrangThaiTienDo: string" vertex="1">
      <mxGeometry height="140" width="200" y="26" as="geometry" />
    </mxCell>
    <mxCell id="LcXY-409vzfgchgwM_3A-3" parent="LcXY-409vzfgchgwM_3A-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="200" y="166" as="geometry" />
    </mxCell>
    <mxCell id="LcXY-409vzfgchgwM_3A-4" parent="LcXY-409vzfgchgwM_3A-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ baoCaoTienDo(): void&lt;br&gt;+ duyetBaoCao(): void&lt;br&gt;+ tuChoiBaoCao(): void&lt;br&gt;+ uploadFileTienDo(): void" vertex="1">
      <mxGeometry height="80" width="200" y="174" as="geometry" />
    </mxCell>
    <mxCell id="QeKxYNgZSGp9Emk5VGAc-6" edge="1" parent="1" source="QeKxYNgZSGp9Emk5VGAc-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;dashed=1;">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="-90" y="860" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="QeKxYNgZSGp9Emk5VGAc-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NhanVienDuAn" vertex="1">
      <mxGeometry height="86" width="200" x="-193.91" y="906" as="geometry">
        <mxRectangle height="30" width="130" x="520" y="530" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="QeKxYNgZSGp9Emk5VGAc-2" parent="QeKxYNgZSGp9Emk5VGAc-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ NgayThamGiaDuAn: DateTime&lt;br&gt;+ VaiTroTrongDuAn: string" vertex="1">
      <mxGeometry height="44" width="200" y="26" as="geometry" />
    </mxCell>
    <mxCell id="QeKxYNgZSGp9Emk5VGAc-3" parent="QeKxYNgZSGp9Emk5VGAc-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="200" y="70" as="geometry" />
    </mxCell>
    <mxCell id="QeKxYNgZSGp9Emk5VGAc-4" parent="QeKxYNgZSGp9Emk5VGAc-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="8" width="200" y="78" as="geometry" />
    </mxCell>
    <mxCell id="Jqhau5jHDxXsKYdf8Zkh-6" edge="1" parent="1" source="Jqhau5jHDxXsKYdf8Zkh-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="930" y="970" />
          <mxPoint x="930" y="970" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-4" connectable="0" parent="Jqhau5jHDxXsKYdf8Zkh-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8592" y="1" as="geometry">
        <mxPoint x="13" y="2" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-5" connectable="0" parent="Jqhau5jHDxXsKYdf8Zkh-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9113" y="-2" as="geometry">
        <mxPoint x="22" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-6" connectable="0" parent="Jqhau5jHDxXsKYdf8Zkh-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=90;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0324" y="2" as="geometry">
        <mxPoint x="18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-6" edge="1" parent="1" source="G1Bnz5JsvCSpgI5tl59X-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;dashed=1;endArrow=none;endFill=0;">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="-1132" y="1540" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="PhanCongCongViec" vertex="1">
      <mxGeometry height="138" width="195" x="-1230" y="1620" as="geometry">
        <mxRectangle height="30" width="150" x="680" y="1124.47" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-2" parent="G1Bnz5JsvCSpgI5tl59X-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaNguoiDung: int&lt;br&gt;+ MaCongViec: int&lt;br&gt;+ NgayGiaoViec: DateTime" vertex="1">
      <mxGeometry height="60" width="195" y="26" as="geometry" />
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-3" parent="G1Bnz5JsvCSpgI5tl59X-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="195" y="86" as="geometry" />
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-4" parent="G1Bnz5JsvCSpgI5tl59X-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ phanCong(): void&lt;br/&gt;+ huyPhanCong(): void" vertex="1">
      <mxGeometry height="44" width="195" y="94" as="geometry" />
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-12" edge="1" parent="1" source="G1Bnz5JsvCSpgI5tl59X-7" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="FGaaYxJouGr9IegEVFHG-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-1505" y="1440" />
          <mxPoint x="-1505" y="1440" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-41" connectable="0" parent="G1Bnz5JsvCSpgI5tl59X-12" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8333" as="geometry">
        <mxPoint x="15" y="8" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-42" connectable="0" parent="G1Bnz5JsvCSpgI5tl59X-12" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.844" y="2" as="geometry">
        <mxPoint x="23" y="1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-43" connectable="0" parent="G1Bnz5JsvCSpgI5tl59X-12" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.1903" y="-4" as="geometry">
        <mxPoint x="-11" y="-1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-7" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NhatKyPhanCongCongViec" vertex="1">
      <mxGeometry height="114" width="260" x="-1535" y="1290" as="geometry">
        <mxRectangle height="30" width="120" x="-205" y="830.0000000000001" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-8" parent="G1Bnz5JsvCSpgI5tl59X-7" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaNhatKyPCCV: int&lt;br&gt;+ HanhDongPCCV: string&lt;br&gt;+ ThoiGianPCCV: DateTime" vertex="1">
      <mxGeometry height="54" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-9" parent="G1Bnz5JsvCSpgI5tl59X-7" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="80" as="geometry" />
    </mxCell>
    <mxCell id="G1Bnz5JsvCSpgI5tl59X-10" parent="G1Bnz5JsvCSpgI5tl59X-7" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ ghiLog(): void" vertex="1">
      <mxGeometry height="26" width="260" y="88" as="geometry" />
    </mxCell>
    <mxCell id="I8oKaqVRjwUwurzyqpso-5" edge="1" parent="1" source="I8oKaqVRjwUwurzyqpso-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="LcXY-409vzfgchgwM_3A-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="kW6yUhoyHavPNyB8MbnI-18" connectable="0" parent="I8oKaqVRjwUwurzyqpso-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;rotation=-60;" value="◀ thuộc" vertex="1">
      <mxGeometry relative="1" x="-0.0602" as="geometry">
        <mxPoint x="21" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-72" connectable="0" parent="I8oKaqVRjwUwurzyqpso-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7088" y="6" as="geometry">
        <mxPoint x="11" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-73" connectable="0" parent="I8oKaqVRjwUwurzyqpso-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8553" y="5" as="geometry">
        <mxPoint x="21" y="3" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="I8oKaqVRjwUwurzyqpso-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="FileTienDoCongViec" vertex="1">
      <mxGeometry height="178" width="180" x="-1610" y="970" as="geometry">
        <mxRectangle height="30" width="130" x="-350" y="700" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="I8oKaqVRjwUwurzyqpso-2" parent="I8oKaqVRjwUwurzyqpso-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaFileTDCV: int&lt;br&gt;+ TenFileTDCV: string&lt;br&gt;+ DuongDanFileTDCV: string&lt;br&gt;+ NgayUploadFileTDCV: DateTime&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="144" width="180" y="26" as="geometry" />
    </mxCell>
    <mxCell id="I8oKaqVRjwUwurzyqpso-3" parent="I8oKaqVRjwUwurzyqpso-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="180" y="170" as="geometry" />
    </mxCell>
    <mxCell id="fArl9yvIIE_HflvjUv1i-5" edge="1" parent="1" source="fArl9yvIIE_HflvjUv1i-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="FGaaYxJouGr9IegEVFHG-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="fArl9yvIIE_HflvjUv1i-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="MucDoUuTien" vertex="1">
      <mxGeometry height="102" width="140" x="-1350" y="2674" as="geometry">
        <mxRectangle height="30" width="120" x="190" y="1380" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="fArl9yvIIE_HflvjUv1i-2" parent="fArl9yvIIE_HflvjUv1i-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaMucDo: int&lt;br&gt;+ TenMucDo: string&lt;br&gt;+ MoTaMucDo: string" vertex="1">
      <mxGeometry height="60" width="140" y="26" as="geometry" />
    </mxCell>
    <mxCell id="fArl9yvIIE_HflvjUv1i-3" parent="fArl9yvIIE_HflvjUv1i-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="140" y="86" as="geometry" />
    </mxCell>
    <mxCell id="fArl9yvIIE_HflvjUv1i-4" parent="fArl9yvIIE_HflvjUv1i-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="8" width="140" y="94" as="geometry" />
    </mxCell>
    <mxCell id="7tdGLAoMYjWFRLLqB3p7-4" edge="1" parent="1" source="7tdGLAoMYjWFRLLqB3p7-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="FGaaYxJouGr9IegEVFHG-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-63" connectable="0" parent="7tdGLAoMYjWFRLLqB3p7-4" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7815" y="-1" as="geometry">
        <mxPoint x="7" y="-23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-64" connectable="0" parent="7tdGLAoMYjWFRLLqB3p7-4" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.6622" as="geometry">
        <mxPoint y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-65" connectable="0" parent="7tdGLAoMYjWFRLLqB3p7-4" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Thuộc ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0246" as="geometry">
        <mxPoint y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="7tdGLAoMYjWFRLLqB3p7-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="FileCongViec" vertex="1">
      <mxGeometry height="168" width="180" x="-2010" y="1824" as="geometry">
        <mxRectangle height="30" width="130" x="-350" y="700" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="7tdGLAoMYjWFRLLqB3p7-2" parent="7tdGLAoMYjWFRLLqB3p7-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaFileCV: int&lt;br&gt;+ TenFileCV: string&lt;br&gt;+ DuongDanFileCV: string&lt;br&gt;+ NgayUploadFileCV: DateTime&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="134" width="180" y="26" as="geometry" />
    </mxCell>
    <mxCell id="7tdGLAoMYjWFRLLqB3p7-3" parent="7tdGLAoMYjWFRLLqB3p7-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="180" y="160" as="geometry" />
    </mxCell>
    <mxCell id="AYw7FUmVlIs3iUGZHeOn-8" edge="1" parent="1" source="yhDT0zH1PYg4IR5jnH_h-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-39" connectable="0" parent="AYw7FUmVlIs3iUGZHeOn-8" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8345" y="1" as="geometry">
        <mxPoint x="8" y="-23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-40" connectable="0" parent="AYw7FUmVlIs3iUGZHeOn-8" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.904" y="2" as="geometry">
        <mxPoint x="8" y="-20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="yhDT0zH1PYg4IR5jnH_h-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="Team" vertex="1">
      <mxGeometry height="220" width="190" x="172" y="906" as="geometry">
        <mxRectangle height="30" width="70" x="-390" y="500" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="yhDT0zH1PYg4IR5jnH_h-2" parent="yhDT0zH1PYg4IR5jnH_h-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaTeam: int&lt;br&gt;+ TenTeam: string&lt;br&gt;+ MoTaTeam: string&lt;br&gt;+ NgayLapTeam: DateTime&lt;br&gt;+ TrangThaiTeam: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="134" width="190" y="26" as="geometry" />
    </mxCell>
    <mxCell id="yhDT0zH1PYg4IR5jnH_h-3" parent="yhDT0zH1PYg4IR5jnH_h-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="190" y="160" as="geometry" />
    </mxCell>
    <mxCell id="yhDT0zH1PYg4IR5jnH_h-4" parent="yhDT0zH1PYg4IR5jnH_h-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoTeam(tenTeam: string): int&lt;br/&gt;+ boNhiemLeader(maNhanVien: int): void" vertex="1">
      <mxGeometry height="52" width="190" y="168" as="geometry" />
    </mxCell>
    <mxCell id="AYw7FUmVlIs3iUGZHeOn-5" edge="1" parent="1" source="AYw7FUmVlIs3iUGZHeOn-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;dashed=1;">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="-245" y="1060" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="AYw7FUmVlIs3iUGZHeOn-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NhanVienTeam" vertex="1">
      <mxGeometry height="106" width="190" x="-340" y="1121" as="geometry">
        <mxRectangle height="30" width="120" x="30" y="700" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="AYw7FUmVlIs3iUGZHeOn-2" parent="AYw7FUmVlIs3iUGZHeOn-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ VaiTroTrongTeam: string&lt;br&gt;+ NgayThamGiaTeam: DateTime&lt;br&gt;+ IsLeader: bool" vertex="1">
      <mxGeometry height="64" width="190" y="26" as="geometry" />
    </mxCell>
    <mxCell id="AYw7FUmVlIs3iUGZHeOn-3" parent="AYw7FUmVlIs3iUGZHeOn-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="190" y="90" as="geometry" />
    </mxCell>
    <mxCell id="AYw7FUmVlIs3iUGZHeOn-4" parent="AYw7FUmVlIs3iUGZHeOn-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="8" width="190" y="98" as="geometry" />
    </mxCell>
    <mxCell id="MRNTbgXyWDwwD3sawcMx-5" edge="1" parent="1" source="MRNTbgXyWDwwD3sawcMx-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;dashed=1;">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="520" y="1240" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="MRNTbgXyWDwwD3sawcMx-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="TeamDuAn" vertex="1">
      <mxGeometry height="76" width="210" x="516" y="984" as="geometry">
        <mxRectangle height="30" width="100" x="5" y="1360" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="MRNTbgXyWDwwD3sawcMx-2" parent="MRNTbgXyWDwwD3sawcMx-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ NgayTeamThamGiaDA: DateTime" vertex="1">
      <mxGeometry height="34" width="210" y="26" as="geometry" />
    </mxCell>
    <mxCell id="MRNTbgXyWDwwD3sawcMx-3" parent="MRNTbgXyWDwwD3sawcMx-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="210" y="60" as="geometry" />
    </mxCell>
    <mxCell id="MRNTbgXyWDwwD3sawcMx-4" parent="MRNTbgXyWDwwD3sawcMx-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="8" width="210" y="68" as="geometry" />
    </mxCell>
    <mxCell id="wVpjQ2P8cBYgM3cTY1Va-6" edge="1" parent="1" source="wVpjQ2P8cBYgM3cTY1Va-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-37" connectable="0" parent="wVpjQ2P8cBYgM3cTY1Va-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8979" y="4" as="geometry">
        <mxPoint y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-38" connectable="0" parent="wVpjQ2P8cBYgM3cTY1Va-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9184" y="4" as="geometry">
        <mxPoint x="6" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-113" connectable="0" parent="wVpjQ2P8cBYgM3cTY1Va-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=15;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.1392" y="5" as="geometry">
        <mxPoint x="13" y="-12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="wVpjQ2P8cBYgM3cTY1Va-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NhatKyDuAn" vertex="1">
      <mxGeometry height="124" width="200" x="-20" y="1259" as="geometry">
        <mxRectangle height="30" width="110" x="-750" y="1276.1400000000003" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="wVpjQ2P8cBYgM3cTY1Va-2" parent="wVpjQ2P8cBYgM3cTY1Va-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaNhatKyTeamDA: int&lt;br&gt;+ HanhDongNKDA: string&lt;br&gt;+ ThoiGianNKDA: DateTime" vertex="1">
      <mxGeometry height="64" width="200" y="26" as="geometry" />
    </mxCell>
    <mxCell id="wVpjQ2P8cBYgM3cTY1Va-3" parent="wVpjQ2P8cBYgM3cTY1Va-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="200" y="90" as="geometry" />
    </mxCell>
    <mxCell id="wVpjQ2P8cBYgM3cTY1Va-4" parent="wVpjQ2P8cBYgM3cTY1Va-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ ghiLog(): void" vertex="1">
      <mxGeometry height="26" width="200" y="98" as="geometry" />
    </mxCell>
    <mxCell id="wVpjQ2P8cBYgM3cTY1Va-7" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="FileDuAn" vertex="1">
      <mxGeometry height="184" width="180" x="100" y="2190" as="geometry">
        <mxRectangle height="30" width="130" x="-350" y="700" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="wVpjQ2P8cBYgM3cTY1Va-8" parent="wVpjQ2P8cBYgM3cTY1Va-7" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaFileDA: int&lt;br&gt;+ TenFileDA: string&lt;br&gt;+ DuongDanFileDA: string&lt;br&gt;+ NgayUploadFileDA: DateTime&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="150" width="180" y="26" as="geometry" />
    </mxCell>
    <mxCell id="wVpjQ2P8cBYgM3cTY1Va-9" parent="wVpjQ2P8cBYgM3cTY1Va-7" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="180" y="176" as="geometry" />
    </mxCell>
    <mxCell id="wVpjQ2P8cBYgM3cTY1Va-10" edge="1" parent="1" source="enBTkT7pxzC_SMcUWDWl-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="wVpjQ2P8cBYgM3cTY1Va-7">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="5xCHvEqshszrvCJ6u1h0-5" edge="1" parent="1" source="5xCHvEqshszrvCJ6u1h0-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="O05rKdK9spuyOT-CUAnu-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-11" connectable="0" parent="5xCHvEqshszrvCJ6u1h0-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=30;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;Đề xuất ▶&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0479" y="-4" as="geometry">
        <mxPoint x="10" y="-11" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-12" connectable="0" parent="5xCHvEqshszrvCJ6u1h0-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8751" as="geometry">
        <mxPoint x="-15" y="-26" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-13" connectable="0" parent="5xCHvEqshszrvCJ6u1h0-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.816" y="-3" as="geometry">
        <mxPoint x="10" y="-16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9ISySmeXkRdA2qAWNLh6-1" edge="1" parent="1" source="5xCHvEqshszrvCJ6u1h0-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="y--h2MhFFpbY8mDH8gJM-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="5xCHvEqshszrvCJ6u1h0-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NganSach" vertex="1">
      <mxGeometry height="310" width="260" x="-70" y="1753" as="geometry">
        <mxRectangle height="30" width="80" x="1250" y="1680" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="5xCHvEqshszrvCJ6u1h0-2" parent="5xCHvEqshszrvCJ6u1h0-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaNganSach: int&lt;br&gt;+ NganSach: decimal&lt;br&gt;+ Version: int&lt;br&gt;+ IsActive: bool&lt;br&gt;+ MoTaNganSach: string&lt;br&gt;+ NgayCapNhatNganSach: DateTime&lt;br&gt;+ NgayDuyetNganSach: DateTime&lt;br&gt;+ TrangThaiNganSach: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="214" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="5xCHvEqshszrvCJ6u1h0-3" parent="5xCHvEqshszrvCJ6u1h0-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="240" as="geometry" />
    </mxCell>
    <mxCell id="5xCHvEqshszrvCJ6u1h0-4" parent="5xCHvEqshszrvCJ6u1h0-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoNganSach(): void&lt;br&gt;+ deXuatDieuChinh(): void&lt;br&gt;+ duyetNganSach(): void" vertex="1">
      <mxGeometry height="62" width="260" y="248" as="geometry" />
    </mxCell>
    <mxCell id="5xCHvEqshszrvCJ6u1h0-6" edge="1" parent="1" source="5xCHvEqshszrvCJ6u1h0-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-28" connectable="0" parent="5xCHvEqshszrvCJ6u1h0-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8376" y="3" as="geometry">
        <mxPoint x="-14" y="-12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-29" connectable="0" parent="5xCHvEqshszrvCJ6u1h0-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8932" y="2" as="geometry">
        <mxPoint x="-19" y="-12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-30" connectable="0" parent="5xCHvEqshszrvCJ6u1h0-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-25;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Thuộc ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0848" y="1" as="geometry">
        <mxPoint y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="LKcM5z-ubJVyyam-9tI6-5" edge="1" parent="1" source="LKcM5z-ubJVyyam-9tI6-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="5xCHvEqshszrvCJ6u1h0-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-79" connectable="0" parent="LKcM5z-ubJVyyam-9tI6-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.74" y="2" as="geometry">
        <mxPoint x="3" y="-14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-80" connectable="0" parent="LKcM5z-ubJVyyam-9tI6-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7288" y="-1" as="geometry">
        <mxPoint y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-81" connectable="0" parent="LKcM5z-ubJVyyam-9tI6-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Thuộc ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.054" y="3" as="geometry">
        <mxPoint x="3" y="-14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="LKcM5z-ubJVyyam-9tI6-6" edge="1" parent="1" source="LKcM5z-ubJVyyam-9tI6-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="FGaaYxJouGr9IegEVFHG-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-44" connectable="0" parent="LKcM5z-ubJVyyam-9tI6-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9282" y="1" as="geometry">
        <mxPoint y="-16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-45" connectable="0" parent="LKcM5z-ubJVyyam-9tI6-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9094" y="-3" as="geometry">
        <mxPoint x="8" y="-14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-46" connectable="0" parent="LKcM5z-ubJVyyam-9tI6-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.1239" y="3" as="geometry">
        <mxPoint y="-21" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="NvCRhkCUS0AH-Iu4hbs--5" edge="1" parent="1" source="LKcM5z-ubJVyyam-9tI6-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="NvCRhkCUS0AH-Iu4hbs--1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="LKcM5z-ubJVyyam-9tI6-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="ChiPhi" vertex="1">
      <mxGeometry height="260" width="260" x="-581.91" y="1777" as="geometry">
        <mxRectangle height="30" width="90" x="-140" y="1796.14" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="LKcM5z-ubJVyyam-9tI6-2" parent="LKcM5z-ubJVyyam-9tI6-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaChiPhi: int&lt;br&gt;+ NoiDungChiPhi: string&lt;br&gt;+ SoTienDaChi: decimal&lt;br&gt;+ NgayChi: DateTime&lt;br&gt;+ TrangThaiChiPhi: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="164" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="LKcM5z-ubJVyyam-9tI6-3" parent="LKcM5z-ubJVyyam-9tI6-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="190" as="geometry" />
    </mxCell>
    <mxCell id="LKcM5z-ubJVyyam-9tI6-4" parent="LKcM5z-ubJVyyam-9tI6-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ themChiPhi(): void&lt;br&gt;+ capNhatChiPhi(): void&lt;br&gt;+ xoaChiPhi(): void" vertex="1">
      <mxGeometry height="62" width="260" y="198" as="geometry" />
    </mxCell>
    <mxCell id="9ISySmeXkRdA2qAWNLh6-3" edge="1" parent="1" source="y--h2MhFFpbY8mDH8gJM-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-31" connectable="0" parent="9ISySmeXkRdA2qAWNLh6-3" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8307" y="3" as="geometry">
        <mxPoint y="-11" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-32" connectable="0" parent="9ISySmeXkRdA2qAWNLh6-3" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.775" y="-1" as="geometry">
        <mxPoint x="-1" y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-33" connectable="0" parent="9ISySmeXkRdA2qAWNLh6-3" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0519" y="-1" as="geometry">
        <mxPoint x="-1" y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="y--h2MhFFpbY8mDH8gJM-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NhatKyNganSach" vertex="1">
      <mxGeometry height="184" width="200" x="290" y="1465" as="geometry">
        <mxRectangle height="30" width="120" x="-360" y="1666.14" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="y--h2MhFFpbY8mDH8gJM-2" parent="y--h2MhFFpbY8mDH8gJM-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaNhatKyNS: int&lt;br&gt;+ SoTienNKNS: decimal&lt;br&gt;+ NganSachTruoc: decimal&lt;br&gt;+ NganSachSau: decimal&lt;br&gt;+ NkNgayCapNhatNS: DateTime&lt;br&gt;+ NkTrangThaiNganSach: string&lt;br&gt;+ HanhDongNKNS: string&lt;br&gt;+ ThoiGianNKNS: DateTime" vertex="1">
      <mxGeometry height="124" width="200" y="26" as="geometry" />
    </mxCell>
    <mxCell id="y--h2MhFFpbY8mDH8gJM-3" parent="y--h2MhFFpbY8mDH8gJM-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="200" y="150" as="geometry" />
    </mxCell>
    <mxCell id="y--h2MhFFpbY8mDH8gJM-4" parent="y--h2MhFFpbY8mDH8gJM-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ ghiLog(): void" vertex="1">
      <mxGeometry height="26" width="200" y="158" as="geometry" />
    </mxCell>
    <mxCell id="hnMNz6sLJDRykfnXpEXb-5" edge="1" parent="1" source="LE_D31fqEWLUAxVKmXNx-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="hnMNz6sLJDRykfnXpEXb-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="2240" y="1200" />
          <mxPoint x="2240" y="1200" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-120" connectable="0" parent="hnMNz6sLJDRykfnXpEXb-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7904" y="-2" as="geometry">
        <mxPoint x="8" y="1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-121" connectable="0" parent="hnMNz6sLJDRykfnXpEXb-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7934" y="-2" as="geometry">
        <mxPoint x="28" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-122" connectable="0" parent="hnMNz6sLJDRykfnXpEXb-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;◀ thuộc&amp;nbsp;&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0076" y="-2" as="geometry">
        <mxPoint x="18" y="1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="LE_D31fqEWLUAxVKmXNx-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="PhongChat" vertex="1">
      <mxGeometry height="190" width="220" x="2190" y="1465" as="geometry">
        <mxRectangle height="30" width="100" x="1220" y="1580" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="LE_D31fqEWLUAxVKmXNx-2" parent="LE_D31fqEWLUAxVKmXNx-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaPhongChat: int&lt;br&gt;+ TenPhong: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="94" width="220" y="26" as="geometry" />
    </mxCell>
    <mxCell id="LE_D31fqEWLUAxVKmXNx-3" parent="LE_D31fqEWLUAxVKmXNx-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="220" y="120" as="geometry" />
    </mxCell>
    <mxCell id="LE_D31fqEWLUAxVKmXNx-4" parent="LE_D31fqEWLUAxVKmXNx-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoPhongChat(): void&lt;br&gt;+ guiTinNhan(): void&lt;br&gt;+ themThanhVienPhong(): void" vertex="1">
      <mxGeometry height="62" width="220" y="128" as="geometry" />
    </mxCell>
    <mxCell id="hnMNz6sLJDRykfnXpEXb-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="TinNhan" vertex="1">
      <mxGeometry height="182" width="180" x="2120" y="888" as="geometry">
        <mxRectangle height="30" width="90" x="-1060" y="540" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="hnMNz6sLJDRykfnXpEXb-2" parent="hnMNz6sLJDRykfnXpEXb-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaTinNhan: int&lt;br&gt;+ NoiDungTinNhan: string&lt;br&gt;+ ThoiGianGui: DateTime&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="104" width="180" y="26" as="geometry" />
    </mxCell>
    <mxCell id="hnMNz6sLJDRykfnXpEXb-3" parent="hnMNz6sLJDRykfnXpEXb-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="180" y="130" as="geometry" />
    </mxCell>
    <mxCell id="hnMNz6sLJDRykfnXpEXb-4" parent="hnMNz6sLJDRykfnXpEXb-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ gui(): void&lt;br/&gt;+ thuHoi(): void" vertex="1">
      <mxGeometry height="44" width="180" y="138" as="geometry" />
    </mxCell>
    <mxCell id="zMm8FlYmg9bQTv8ymNP0-6" edge="1" parent="1" source="zMm8FlYmg9bQTv8ymNP0-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;dashed=1;endArrow=none;endFill=0;">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="1520" y="150" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="zMm8FlYmg9bQTv8ymNP0-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="ThanhVienPhongChat" vertex="1">
      <mxGeometry height="68" width="260" x="1390" y="-20" as="geometry">
        <mxRectangle height="30" width="160" x="1540" y="840" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="zMm8FlYmg9bQTv8ymNP0-2" parent="zMm8FlYmg9bQTv8ymNP0-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ VaiTroTrongPhongChat: string" vertex="1">
      <mxGeometry height="34" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="zMm8FlYmg9bQTv8ymNP0-3" parent="zMm8FlYmg9bQTv8ymNP0-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="60" as="geometry" />
    </mxCell>
    <mxCell id="zMm8FlYmg9bQTv8ymNP0-5" edge="1" parent="1" source="xWTXjU9PBJvNTCzDRvnS-1" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="LE_D31fqEWLUAxVKmXNx-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="2360" y="150" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-128" connectable="0" parent="zMm8FlYmg9bQTv8ymNP0-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9635" y="6" as="geometry">
        <mxPoint x="14" y="18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-129" connectable="0" parent="zMm8FlYmg9bQTv8ymNP0-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9796" y="4" as="geometry">
        <mxPoint y="-16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="5grzo2p5ejAfA8UIloDJ-6" edge="1" parent="1" source="5grzo2p5ejAfA8UIloDJ-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="O05rKdK9spuyOT-CUAnu-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="H2kTQfWC1UXGSywkNDVl-84" connectable="0" parent="5grzo2p5ejAfA8UIloDJ-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=60;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;cho ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0036" y="-2" as="geometry">
        <mxPoint x="15" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-2" connectable="0" parent="5grzo2p5ejAfA8UIloDJ-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8839" as="geometry">
        <mxPoint x="18" y="-20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-3" connectable="0" parent="5grzo2p5ejAfA8UIloDJ-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8958" y="3" as="geometry">
        <mxPoint x="5" y="-7" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="8xK73OlVaXU_7q94dwpZ-1" edge="1" parent="1" source="5grzo2p5ejAfA8UIloDJ-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="850" y="820" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-1" connectable="0" parent="8xK73OlVaXU_7q94dwpZ-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9756" y="6" as="geometry">
        <mxPoint x="11" y="-1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-2" connectable="0" parent="8xK73OlVaXU_7q94dwpZ-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;dựa vào ▶&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.1736" as="geometry">
        <mxPoint y="-20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-3" connectable="0" parent="8xK73OlVaXU_7q94dwpZ-1" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9728" y="4" as="geometry">
        <mxPoint y="-14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="5grzo2p5ejAfA8UIloDJ-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="DanhGiaNhanVien" vertex="1">
      <mxGeometry height="330" width="260" x="-1390" y="676" as="geometry">
        <mxRectangle height="30" width="140" x="780" y="472" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="5grzo2p5ejAfA8UIloDJ-2" parent="5grzo2p5ejAfA8UIloDJ-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaDanhGiaNhanVien: int&lt;br&gt;+ DiemTongDanhGiaNV: int&lt;br&gt;+ NgayDanhGiaNV: DateTime&lt;br&gt;+ XepLoai: string&lt;br&gt;+ NhanXetTongQuanNV: string&lt;br&gt;+ TrangThaiDanhGiaNV: string&lt;br&gt;+ MaNguoiDungDuyet: int&lt;br&gt;+ NgayDuyetDanhGiaNV: DateTime&lt;br&gt;+ LyDoTuChoiDanhGiaNV: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="234" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="5grzo2p5ejAfA8UIloDJ-3" parent="5grzo2p5ejAfA8UIloDJ-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="260" as="geometry" />
    </mxCell>
    <mxCell id="5grzo2p5ejAfA8UIloDJ-4" parent="5grzo2p5ejAfA8UIloDJ-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoDanhGiaNhanVien(): void&lt;br&gt;+ capNhatDanhGia(): void&lt;br&gt;+ duyetDanhGia(): void" vertex="1">
      <mxGeometry height="62" width="260" y="268" as="geometry" />
    </mxCell>
    <mxCell id="3QHnkx9HTkC5YB1tlKDT-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="TieuChiDanhGia" vertex="1">
      <mxGeometry height="156" width="155" x="-2085" y="384" as="geometry">
        <mxRectangle height="30" width="130" x="-880" y="560" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="3QHnkx9HTkC5YB1tlKDT-2" parent="3QHnkx9HTkC5YB1tlKDT-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaTieuChi: int&lt;br&gt;+ TenTieuChi: string&lt;br&gt;+ DiemTieuChi: double&lt;br&gt;+ MoTa: string&lt;br&gt;+ LoaiTieuChi: string&lt;br&gt;+ TrangThaiTieuChi: string" vertex="1">
      <mxGeometry height="114" width="155" y="26" as="geometry" />
    </mxCell>
    <mxCell id="3QHnkx9HTkC5YB1tlKDT-3" parent="3QHnkx9HTkC5YB1tlKDT-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="155" y="140" as="geometry" />
    </mxCell>
    <mxCell id="3QHnkx9HTkC5YB1tlKDT-4" parent="3QHnkx9HTkC5YB1tlKDT-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="8" width="155" y="148" as="geometry" />
    </mxCell>
    <mxCell id="hSfZnnZLLSagc-6fNAt5-6" edge="1" parent="1" source="hSfZnnZLLSagc-6fNAt5-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="FGaaYxJouGr9IegEVFHG-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-2400" y="960" />
          <mxPoint x="-2400" y="2770" />
          <mxPoint x="-1600" y="2770" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="H2kTQfWC1UXGSywkNDVl-48" connectable="0" parent="hSfZnnZLLSagc-6fNAt5-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0705" y="-4" as="geometry">
        <mxPoint x="23" y="-8" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-59" connectable="0" parent="hSfZnnZLLSagc-6fNAt5-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9779" as="geometry">
        <mxPoint x="13" y="-1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-60" connectable="0" parent="hSfZnnZLLSagc-6fNAt5-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9717" as="geometry">
        <mxPoint x="2" y="-19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-70" edge="1" parent="1" source="hSfZnnZLLSagc-6fNAt5-1" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="5grzo2p5ejAfA8UIloDJ-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-1650" y="920" />
          <mxPoint x="-1650" y="920" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-106" connectable="0" parent="G3jGO_3h6dEc7QatDs8r-70" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8639" y="2" as="geometry">
        <mxPoint y="-12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-107" connectable="0" parent="G3jGO_3h6dEc7QatDs8r-70" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8302" as="geometry">
        <mxPoint y="-20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-108" connectable="0" parent="G3jGO_3h6dEc7QatDs8r-70" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0291" y="2" as="geometry">
        <mxPoint y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="hSfZnnZLLSagc-6fNAt5-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="CtDanhGiaNhanVien" vertex="1">
      <mxGeometry height="156" width="180" x="-2097.5" y="836" as="geometry">
        <mxRectangle height="30" width="150" x="1250" y="490" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="hSfZnnZLLSagc-6fNAt5-2" parent="hSfZnnZLLSagc-6fNAt5-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaChiTietDGNV: int&lt;br&gt;+ NoiDungDanhGiaNhanVien: string&lt;br&gt;+ DiemDanhGiaNV: int&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="114" width="180" y="26" as="geometry" />
    </mxCell>
    <mxCell id="hSfZnnZLLSagc-6fNAt5-3" parent="hSfZnnZLLSagc-6fNAt5-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="180" y="140" as="geometry" />
    </mxCell>
    <mxCell id="hSfZnnZLLSagc-6fNAt5-4" parent="hSfZnnZLLSagc-6fNAt5-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="8" width="180" y="148" as="geometry" />
    </mxCell>
    <mxCell id="nGZGdnT23SMgTuQgHzuC-6" edge="1" parent="1" source="nGZGdnT23SMgTuQgHzuC-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1290" y="1480" />
          <mxPoint x="1290" y="1480" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-13" connectable="0" parent="nGZGdnT23SMgTuQgHzuC-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9209" y="-2" as="geometry">
        <mxPoint x="-1" y="-13" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-14" connectable="0" parent="nGZGdnT23SMgTuQgHzuC-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0.*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8813" y="-2" as="geometry">
        <mxPoint x="9" y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-15" connectable="0" parent="nGZGdnT23SMgTuQgHzuC-6" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.1126" y="-1" as="geometry">
        <mxPoint y="-14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="gmlYSlbSX7t0BqN18J_R-5" edge="1" parent="1" source="nGZGdnT23SMgTuQgHzuC-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="gmlYSlbSX7t0BqN18J_R-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1640" y="1020" />
          <mxPoint x="1640" y="1020" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="kW6yUhoyHavPNyB8MbnI-29" connectable="0" parent="gmlYSlbSX7t0BqN18J_R-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;rotation=-90;" value="có ▶" vertex="1">
      <mxGeometry relative="1" x="0.0626" y="-2" as="geometry">
        <mxPoint x="18" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-98" connectable="0" parent="gmlYSlbSX7t0BqN18J_R-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.728" y="-1" as="geometry">
        <mxPoint x="13" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-99" connectable="0" parent="gmlYSlbSX7t0BqN18J_R-5" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7687" y="-4" as="geometry">
        <mxPoint x="16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="nGZGdnT23SMgTuQgHzuC-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="DanhGiaDuAn" vertex="1">
      <mxGeometry height="338" width="170" x="1610" y="1160" as="geometry">
        <mxRectangle height="30" width="120" x="-930" y="1634.14" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="nGZGdnT23SMgTuQgHzuC-2" parent="nGZGdnT23SMgTuQgHzuC-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaDanhGiaDuAn: int&lt;br&gt;+ DiemTongDanhGiaDA: int&lt;br&gt;+ NhanXetTongDuAn: string&lt;br&gt;+ NgayDanhGiaDA: DateTime&lt;br&gt;+ TrangThaiDanhGiaDA: string&lt;br&gt;+ NgayDuyetDanhGiaDA: DateTime&lt;br&gt;+ LyDoTuChoiDanhGiaDA: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="224" width="170" y="26" as="geometry" />
    </mxCell>
    <mxCell id="nGZGdnT23SMgTuQgHzuC-3" parent="nGZGdnT23SMgTuQgHzuC-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="170" y="250" as="geometry" />
    </mxCell>
    <mxCell id="nGZGdnT23SMgTuQgHzuC-4" parent="nGZGdnT23SMgTuQgHzuC-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoDanhGia(): void&lt;br&gt;+ guiDuyet(): void&lt;br&gt;+ duyetDanhGia(): void&lt;br&gt;+ tuChoiDanhGia(): void" vertex="1">
      <mxGeometry height="80" width="170" y="258" as="geometry" />
    </mxCell>
    <mxCell id="gmlYSlbSX7t0BqN18J_R-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="CtDanhGiaDuAn" vertex="1">
      <mxGeometry height="166" width="140" x="1540" y="714" as="geometry">
        <mxRectangle height="30" width="130" x="-1260" y="1730" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="gmlYSlbSX7t0BqN18J_R-2" parent="gmlYSlbSX7t0BqN18J_R-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaChiTietDGDA: int&lt;br&gt;+ MaDanhGiaDuAn: int&lt;br&gt;+ NhanXetDuAn: string&lt;br&gt;+ MaTieuChi: int&lt;br&gt;+ DiemDanhGiaDA: int&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="124" width="140" y="26" as="geometry" />
    </mxCell>
    <mxCell id="gmlYSlbSX7t0BqN18J_R-3" parent="gmlYSlbSX7t0BqN18J_R-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="140" y="150" as="geometry" />
    </mxCell>
    <mxCell id="gmlYSlbSX7t0BqN18J_R-4" parent="gmlYSlbSX7t0BqN18J_R-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="8" width="140" y="158" as="geometry" />
    </mxCell>
    <mxCell id="vUZi053HICLdzrY2CnxL-5" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="ChucDanh" vertex="1">
      <mxGeometry height="102" width="150" x="523" y="-240" as="geometry">
        <mxRectangle height="30" width="130" x="-880" y="560" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="vUZi053HICLdzrY2CnxL-6" parent="vUZi053HICLdzrY2CnxL-5" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaChucDanh: int&lt;br&gt;+ TenChucDanh: string&lt;br&gt;+ MoTaChucDanh: string" vertex="1">
      <mxGeometry height="60" width="150" y="26" as="geometry" />
    </mxCell>
    <mxCell id="vUZi053HICLdzrY2CnxL-7" parent="vUZi053HICLdzrY2CnxL-5" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="150" y="86" as="geometry" />
    </mxCell>
    <mxCell id="vUZi053HICLdzrY2CnxL-8" parent="vUZi053HICLdzrY2CnxL-5" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="8" width="150" y="94" as="geometry" />
    </mxCell>
    <mxCell id="NvCRhkCUS0AH-Iu4hbs--1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NhatKyChiPhi" vertex="1">
      <mxGeometry height="210" width="220" x="-1160" y="1980" as="geometry">
        <mxRectangle height="30" width="120" x="-360" y="1666.14" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="NvCRhkCUS0AH-Iu4hbs--2" parent="NvCRhkCUS0AH-Iu4hbs--1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaNhatKyCP: int&lt;br&gt;+ MaCongViec: int&lt;br&gt;+ MaChiPhi: int&lt;br&gt;+ NkSoTienDaChi: decimal&lt;br&gt;+ NkNgayChi: DateTime&lt;br&gt;+ NkTrangThaiChiPhi: string&lt;br&gt;+ HanhDongNKCP: string&lt;br&gt;+ ThoiGianNKCP: DateTime" vertex="1">
      <mxGeometry height="150" width="220" y="26" as="geometry" />
    </mxCell>
    <mxCell id="NvCRhkCUS0AH-Iu4hbs--3" parent="NvCRhkCUS0AH-Iu4hbs--1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="220" y="176" as="geometry" />
    </mxCell>
    <mxCell id="NvCRhkCUS0AH-Iu4hbs--4" parent="NvCRhkCUS0AH-Iu4hbs--1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ ghiLog(): void" vertex="1">
      <mxGeometry height="26" width="220" y="184" as="geometry" />
    </mxCell>
    <mxCell id="autogen-2" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="AiDataset" vertex="1">
      <mxGeometry height="282" width="260" x="1490" y="2043" as="geometry" />
    </mxCell>
    <mxCell id="autogen-3" parent="autogen-2" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaData: int&lt;br&gt;+ SoNhanVienDuAn: int&lt;br&gt;+ TongSoCongViec: int&lt;br&gt;+ SoCongViecTre: int&lt;br&gt;+ TyLeCongViecTre: double&lt;br&gt;+ ChiPhiDuKien: decimal&lt;br&gt;+ ChiPhiThucTe: decimal&lt;br&gt;+ ChenhLechChiPhi: decimal&lt;br&gt;+ SoLanThayDoiNhanSu: int&lt;br&gt;+ SoLanThayDoiQuanLy: int&lt;br&gt;+ SoNgayTreTienDo: int&lt;br&gt;+ IsTre: bool" vertex="1">
      <mxGeometry height="204" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-4" parent="autogen-2" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="230" as="geometry" />
    </mxCell>
    <mxCell id="autogen-5" parent="autogen-2" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ tongHopDataset(): void&lt;br&gt;+ kiemTraChatLuongDataset(): void" vertex="1">
      <mxGeometry height="44" width="260" y="238" as="geometry" />
    </mxCell>
    <mxCell id="autogen-6" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="AiKetQua" vertex="1">
      <mxGeometry height="160" width="260" x="1980" y="2104" as="geometry" />
    </mxCell>
    <mxCell id="autogen-7" parent="autogen-6" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaAiKetQua: int&lt;br&gt;+ DoTinCayKetQua: double&lt;br&gt;+ ThoiGianDuDoanKetQua: DateTime" vertex="1">
      <mxGeometry height="64" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-8" parent="autogen-6" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="90" as="geometry" />
    </mxCell>
    <mxCell id="autogen-9" parent="autogen-6" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ duDoanTreHan(): bool&lt;br&gt;+ duDoanNguyenNhan(): string&lt;br&gt;+ luuKetQuaDuDoan(): void" vertex="1">
      <mxGeometry height="62" width="260" y="98" as="geometry" />
    </mxCell>
    <mxCell id="autogen-10" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="AiModel" vertex="1">
      <mxGeometry height="282" width="260" x="1980" y="2500" as="geometry" />
    </mxCell>
    <mxCell id="autogen-11" parent="autogen-10" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaModel: int&lt;br&gt;+ TenModel: string&lt;br&gt;+ SoLuongDuLieu: int&lt;br&gt;+ DoChinhXac: double&lt;br&gt;+ NgayTao: DateTime&lt;br&gt;+ MoTaModel: string&lt;br&gt;+ LoaiModel: string&lt;br&gt;+ IsActive: bool&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="204" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-12" parent="autogen-10" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="230" as="geometry" />
    </mxCell>
    <mxCell id="autogen-13" parent="autogen-10" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ trainModel(): void&lt;br&gt;+ kichHoatModel(): void" vertex="1">
      <mxGeometry height="44" width="260" y="238" as="geometry" />
    </mxCell>
    <mxCell id="autogen-14" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="AiNguyenNhan" vertex="1">
      <mxGeometry height="154" width="260" x="1080" y="2110" as="geometry" />
    </mxCell>
    <mxCell id="autogen-15" parent="autogen-14" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaAINguyenNhan: int&lt;br&gt;+ DoTinCay: double&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="94" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-16" parent="autogen-14" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="120" as="geometry" />
    </mxCell>
    <mxCell id="autogen-17" parent="autogen-14" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ xacNhanNguyenNhan(): void" vertex="1">
      <mxGeometry height="26" width="260" y="128" as="geometry" />
    </mxCell>
    <mxCell id="autogen-18" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="AspNetUserClaims" vertex="1">
      <mxGeometry height="116" width="260" x="-193.91" y="-510" as="geometry" />
    </mxCell>
    <mxCell id="autogen-19" parent="autogen-18" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ Id: int&lt;br&gt;+ ClaimType: string&lt;br&gt;+ ClaimValue: string" vertex="1">
      <mxGeometry height="78" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-20" parent="autogen-18" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="104" as="geometry" />
    </mxCell>
    <mxCell id="autogen-21" parent="autogen-18" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="4" width="260" y="112" as="geometry" />
    </mxCell>
    <mxCell id="autogen-22" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="AspNetUserLogins" vertex="1">
      <mxGeometry height="116" width="200" x="290" y="-140" as="geometry" />
    </mxCell>
    <mxCell id="autogen-23" parent="autogen-22" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ LoginProvider: string&lt;br&gt;+ ProviderKey: string&lt;br&gt;+ Id: string&lt;br&gt;+ ProviderDisplayName: string" vertex="1">
      <mxGeometry height="78" width="200" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-24" parent="autogen-22" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="200" y="104" as="geometry" />
    </mxCell>
    <mxCell id="autogen-25" parent="autogen-22" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="4" width="200" y="112" as="geometry" />
    </mxCell>
    <mxCell id="autogen-26" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="AspNetUserTokens" vertex="1">
      <mxGeometry height="116" width="260" x="-611.91" y="-433" as="geometry" />
    </mxCell>
    <mxCell id="autogen-27" parent="autogen-26" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ Id: string&lt;br&gt;+ LoginProvider: string&lt;br&gt;+ Name: string&lt;br&gt;+ Value: string" vertex="1">
      <mxGeometry height="78" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-28" parent="autogen-26" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="104" as="geometry" />
    </mxCell>
    <mxCell id="autogen-29" parent="autogen-26" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="4" width="260" y="112" as="geometry" />
    </mxCell>
    <mxCell id="autogen-30" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="DeXuatCongViec" vertex="1">
      <mxGeometry height="290" width="260" x="-890" y="2580" as="geometry" />
    </mxCell>
    <mxCell id="autogen-31" parent="autogen-30" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaDeXuatCV: int&lt;br&gt;+ TenCongViecDeXuat: string&lt;br&gt;+ MoTaCongViecDeXuat: string&lt;br&gt;+ ChiPhiDeXuat: decimal&lt;br&gt;+ NgayBatDauCongViecDeXuat: DateTime&lt;br&gt;+ NgayKetThucCVDeXuatDuKien: DateTime&lt;br&gt;+ NgayDeXuatCongViec: DateTime&lt;br&gt;+ NgayDuyetDeXuatCongViec: DateTime&lt;br&gt;+ TrangThaiCongViecDeXuat: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="194" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-32" parent="autogen-30" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="220" as="geometry" />
    </mxCell>
    <mxCell id="autogen-33" parent="autogen-30" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoDeXuat(): void&lt;br&gt;+ duyetDeXuat(): void&lt;br&gt;+ tuChoiDeXuat(): void" vertex="1">
      <mxGeometry height="62" width="260" y="228" as="geometry" />
    </mxCell>
    <mxCell id="autogen-34" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="DeXuatNganSach" vertex="1">
      <mxGeometry height="242" width="260" x="-680" y="2200" as="geometry" />
    </mxCell>
    <mxCell id="autogen-35" parent="autogen-34" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaDeXuatNS: int&lt;br&gt;+ NganSachCu: decimal&lt;br&gt;+ NganSachDeXuat: decimal&lt;br&gt;+ LyDoDeXuat: string&lt;br&gt;+ NgayDeXuat: DateTime&lt;br&gt;+ NgayDuyet: DateTime&lt;br&gt;+ TrangThaiDeXuat: string&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="164" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-36" parent="autogen-34" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="190" as="geometry" />
    </mxCell>
    <mxCell id="autogen-37" parent="autogen-34" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoDeXuatNganSach(): void&lt;br&gt;+ duyetDeXuatNganSach(): void" vertex="1">
      <mxGeometry height="44" width="260" y="198" as="geometry" />
    </mxCell>
    <mxCell id="autogen-38" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="DmNguyenNhan" vertex="1">
      <mxGeometry height="80" width="260" x="1080" y="2620" as="geometry" />
    </mxCell>
    <mxCell id="autogen-39" parent="autogen-38" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaDMNguyenNhan: int&lt;br&gt;+ TenNguyenNhan: string" vertex="1">
      <mxGeometry height="42" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-40" parent="autogen-38" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="68" as="geometry" />
    </mxCell>
    <mxCell id="autogen-41" parent="autogen-38" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="4" width="260" y="76" as="geometry" />
    </mxCell>
    <mxCell id="autogen-42" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="FileCtCongViec" vertex="1">
      <mxGeometry height="152" width="220" x="-2315" y="2598" as="geometry" />
    </mxCell>
    <mxCell id="autogen-43" parent="autogen-42" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaFileCTCV: int&lt;br&gt;+ TenFileCTCV: string&lt;br&gt;+ DuongDanFileCTCV: string&lt;br&gt;+ NgayUploadFileCTCV: DateTime&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="114" width="220" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-44" parent="autogen-42" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="220" y="140" as="geometry" />
    </mxCell>
    <mxCell id="autogen-45" parent="autogen-42" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="4" width="220" y="148" as="geometry" />
    </mxCell>
    <mxCell id="autogen-46" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="PhanCongCtCongViec" vertex="1">
      <mxGeometry height="82" width="170" x="-1950" y="2343" as="geometry" />
    </mxCell>
    <mxCell id="autogen-47" parent="autogen-46" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ NgayGiaoCTCV: DateTime&lt;br&gt;+ VaiTroTrongCTCV: string" vertex="1">
      <mxGeometry height="44" width="170" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-48" parent="autogen-46" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="170" y="70" as="geometry" />
    </mxCell>
    <mxCell id="autogen-49" parent="autogen-46" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="4" width="170" y="78" as="geometry" />
    </mxCell>
    <mxCell id="autogen-50" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NhatKyPhanCongCtCongViec" vertex="1">
      <mxGeometry height="98" width="190" x="-2300" y="190" as="geometry" />
    </mxCell>
    <mxCell id="autogen-51" parent="autogen-50" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaNhatKyPCCTCV: int&lt;br&gt;+ HanhDongPCCTCV: string&lt;br&gt;+ ThoiGianPCCTCV: DateTime" vertex="1">
      <mxGeometry height="60" width="190" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-52" parent="autogen-50" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="190" y="86" as="geometry" />
    </mxCell>
    <mxCell id="autogen-53" parent="autogen-50" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="4" width="190" y="94" as="geometry" />
    </mxCell>
    <mxCell id="autogen-54" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NhatKyQuanLyDuAn" vertex="1">
      <mxGeometry height="132" width="260" x="1030" y="1040" as="geometry" />
    </mxCell>
    <mxCell id="autogen-55" parent="autogen-54" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaNhatKyQLDA: int&lt;br&gt;+ NkHanhDongQLDA: string&lt;br&gt;+ NkThoiGianQLDA: DateTime&lt;br&gt;+ QLDATuNgay: DateTime&lt;br&gt;+ QLDADenNgay: DateTime" vertex="1">
      <mxGeometry height="94" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-56" parent="autogen-54" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="120" as="geometry" />
    </mxCell>
    <mxCell id="autogen-57" parent="autogen-54" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="" vertex="1">
      <mxGeometry height="4" width="260" y="128" as="geometry" />
    </mxCell>
    <mxCell id="autogen-58" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="YeuCauDoiQuanLy" vertex="1">
      <mxGeometry height="202" width="260" x="1230" y="714" as="geometry" />
    </mxCell>
    <mxCell id="autogen-59" parent="autogen-58" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaYeuCauDoiQuanLy: int&lt;br&gt;+ TrangThaiYeuCauDoiQuanLy: string&lt;br&gt;+ NgayTaoYeuCauDoiQuanLy: DateTime&lt;br&gt;+ NgayDuyetYeuCauDoiQuanLy: DateTime&lt;br&gt;+ IsDeleted: bool&lt;br&gt;+ DeletedAt: DateTime&lt;br&gt;+ DeletedBy: int" vertex="1">
      <mxGeometry height="124" width="260" y="26" as="geometry" />
    </mxCell>
    <mxCell id="autogen-60" parent="autogen-58" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="260" y="150" as="geometry" />
    </mxCell>
    <mxCell id="autogen-61" parent="autogen-58" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ taoYeuCauDoiQuanLy(): void&lt;br&gt;+ duyetYeuCauDoiQuanLy(): void" vertex="1">
      <mxGeometry height="44" width="260" y="158" as="geometry" />
    </mxCell>
    <mxCell id="autogen-62" edge="1" parent="1" source="autogen-2" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1620" y="1690" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-7" connectable="0" parent="autogen-62" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9313" y="-2" as="geometry">
        <mxPoint x="-10" y="-14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-8" connectable="0" parent="autogen-62" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9314" as="geometry">
        <mxPoint x="23" y="-1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-9" connectable="0" parent="autogen-62" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.1097" as="geometry">
        <mxPoint y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-65" edge="1" parent="1" source="autogen-6" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="2130" y="1650" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-2" connectable="0" parent="autogen-65" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9656" y="-2" as="geometry">
        <mxPoint x="-4" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-4" connectable="0" parent="autogen-65" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9519" y="1" as="geometry">
        <mxPoint x="25" y="1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-6" connectable="0" parent="autogen-65" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.2211" y="-1" as="geometry">
        <mxPoint x="-1" y="-19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-68" edge="1" parent="1" source="autogen-6" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="autogen-2">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-94" connectable="0" parent="autogen-68" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7559" y="-1" as="geometry">
        <mxPoint x="-2" y="-19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-96" connectable="0" parent="autogen-68" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8094" y="-1" as="geometry">
        <mxPoint y="-23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-97" connectable="0" parent="autogen-68" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ dựa vào&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.1003" y="-2" as="geometry">
        <mxPoint x="6" y="-12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-71" edge="1" parent="1" source="autogen-6" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="autogen-10">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-88" connectable="0" parent="autogen-71" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7516" y="7" as="geometry">
        <mxPoint x="23" y="-2" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-89" connectable="0" parent="autogen-71" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7086" y="3" as="geometry">
        <mxPoint x="17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-90" connectable="0" parent="autogen-71" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ dựa vào&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0085" y="3" as="geometry">
        <mxPoint x="17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-74" edge="1" parent="1" source="autogen-6" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="autogen-38">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-91" connectable="0" parent="autogen-74" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8934" y="-1" as="geometry">
        <mxPoint x="-23" y="-20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-92" connectable="0" parent="autogen-74" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.895" y="-5" as="geometry">
        <mxPoint x="-7" y="-12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-93" connectable="0" parent="autogen-74" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-30;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ dựa vào&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0328" y="-1" as="geometry">
        <mxPoint y="-22" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-77" edge="1" parent="1" source="autogen-14" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="autogen-38">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-85" connectable="0" parent="autogen-77" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8146" y="2" as="geometry">
        <mxPoint x="18" y="-1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-86" connectable="0" parent="autogen-77" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8431" y="5" as="geometry">
        <mxPoint x="7" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-87" connectable="0" parent="autogen-77" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ dựa vào&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0315" y="5" as="geometry">
        <mxPoint x="15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-80" edge="1" parent="1" source="autogen-14" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1210" y="1730" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-10" connectable="0" parent="autogen-80" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8853" y="-1" as="geometry">
        <mxPoint x="-13" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-11" connectable="0" parent="autogen-80" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8796" y="-4" as="geometry">
        <mxPoint x="21" y="6" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-12" connectable="0" parent="autogen-80" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0767" y="-6" as="geometry">
        <mxPoint x="14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-83" edge="1" parent="1" source="qPm_Ca8Bxiu_PFRpAWqy-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="DHByf7-s3Gl3JD-fI9mn-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="autogen-86" edge="1" parent="1" source="autogen-18" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="GlT-6QYPskowLD9cseYI-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="autogen-87" connectable="0" parent="autogen-86" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="0..*" vertex="1">
      <mxGeometry relative="1" x="0.85" as="geometry">
        <mxPoint x="22" y="-90" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-88" connectable="0" parent="autogen-86" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="1" vertex="1">
      <mxGeometry relative="1" x="-0.85" as="geometry">
        <mxPoint x="14" y="86" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-89" edge="1" parent="1" source="autogen-22" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="GlT-6QYPskowLD9cseYI-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="autogen-90" connectable="0" parent="autogen-89" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="0..*" vertex="1">
      <mxGeometry relative="1" x="0.85" as="geometry">
        <mxPoint x="168" y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-91" connectable="0" parent="autogen-89" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="1" vertex="1">
      <mxGeometry relative="1" x="-0.85" as="geometry">
        <mxPoint x="-184" y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-98" edge="1" parent="1" source="autogen-26" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="GlT-6QYPskowLD9cseYI-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="autogen-99" connectable="0" parent="autogen-98" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="0..*" vertex="1">
      <mxGeometry relative="1" x="0.85" as="geometry">
        <mxPoint x="-122" y="-110" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-100" connectable="0" parent="autogen-98" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="1" vertex="1">
      <mxGeometry relative="1" x="-0.85" as="geometry">
        <mxPoint x="164" y="97" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-104" edge="1" parent="1" source="FGaaYxJouGr9IegEVFHG-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="fArl9yvIIE_HflvjUv1i-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-53" connectable="0" parent="autogen-104" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8916" y="3" as="geometry">
        <mxPoint x="14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-54" connectable="0" parent="autogen-104" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8878" y="1" as="geometry">
        <mxPoint x="31" y="-4" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-55" connectable="0" parent="autogen-104" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=75;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Thuộc ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0832" y="1" as="geometry">
        <mxPoint x="15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-107" edge="1" parent="1" source="FGaaYxJouGr9IegEVFHG-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="autogen-30">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-49" connectable="0" parent="autogen-107" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9209" y="4" as="geometry">
        <mxPoint x="7" y="-10" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-50" connectable="0" parent="autogen-107" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9382" y="1" as="geometry">
        <mxPoint x="24" y="-9" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-52" connectable="0" parent="autogen-107" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=45;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Thuộc ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0476" y="-1" as="geometry">
        <mxPoint x="31" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-113" edge="1" parent="1" source="sSkXnrdGMHA7VeClHVL3-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="FGaaYxJouGr9IegEVFHG-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-61" connectable="0" parent="autogen-113" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8386" as="geometry">
        <mxPoint y="-21" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-62" connectable="0" parent="autogen-113" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.862" y="1" as="geometry">
        <mxPoint y="-26" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-119" edge="1" parent="1" source="gmlYSlbSX7t0BqN18J_R-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1610" y="450" />
          <mxPoint x="-1930" y="450" />
        </Array>
        <mxPoint x="1550" y="450" as="sourcePoint" />
        <mxPoint x="-1930" y="451.9999999999998" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-100" connectable="0" parent="autogen-119" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.982" as="geometry">
        <mxPoint x="20" y="-4" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-101" connectable="0" parent="autogen-119" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9781" y="-1" as="geometry">
        <mxPoint y="-19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-102" connectable="0" parent="autogen-119" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;Dựa vào ▶&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0788" y="-1" as="geometry">
        <mxPoint x="-1" y="-19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-125" edge="1" parent="1" source="hSfZnnZLLSagc-6fNAt5-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="3QHnkx9HTkC5YB1tlKDT-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-103" connectable="0" parent="autogen-125" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7458" y="-3" as="geometry">
        <mxPoint x="15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-104" connectable="0" parent="autogen-125" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7178" y="-1" as="geometry">
        <mxPoint x="27" y="16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-105" connectable="0" parent="autogen-125" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Dựa vào ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0608" y="-3" as="geometry">
        <mxPoint x="15" y="-1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-140" edge="1" parent="1" source="DHByf7-s3Gl3JD-fI9mn-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="AoST3ZVm1FiiXK3rUbv--1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="autogen-141" connectable="0" parent="autogen-140" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="1" vertex="1">
      <mxGeometry relative="1" x="0.85" as="geometry">
        <mxPoint x="25" y="3" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-142" connectable="0" parent="autogen-140" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="1..*" vertex="1">
      <mxGeometry relative="1" x="-0.85" as="geometry">
        <mxPoint x="21" y="-10" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-143" edge="1" parent="1" source="autogen-30" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="MAO-DfN84yR15LXXRfSc-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="500" y="2790" />
          <mxPoint x="500" y="2790" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-158" connectable="0" parent="autogen-143" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9456" y="-1" as="geometry">
        <mxPoint x="6" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-159" connectable="0" parent="autogen-143" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9655" y="2" as="geometry">
        <mxPoint y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-160" connectable="0" parent="autogen-143" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Dựa vào ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.2851" y="3" as="geometry">
        <mxPoint y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-146" edge="1" parent="1" source="autogen-30" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="850" y="2650" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-16" connectable="0" parent="autogen-146" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9634" y="-4" as="geometry">
        <mxPoint x="11" y="-4" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-17" connectable="0" parent="autogen-146" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9751" y="1" as="geometry">
        <mxPoint y="-19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-18" connectable="0" parent="autogen-146" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.198" y="1" as="geometry">
        <mxPoint y="-19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-149" edge="1" parent="1" source="autogen-30" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="fArl9yvIIE_HflvjUv1i-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-135" connectable="0" parent="autogen-149" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8333" y="-2" as="geometry">
        <mxPoint y="-13" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-136" connectable="0" parent="autogen-149" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8056" as="geometry">
        <mxPoint y="-25" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-137" connectable="0" parent="autogen-149" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ thuộc&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0231" y="1" as="geometry">
        <mxPoint y="-16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-152" edge="1" parent="1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="-759" y="2580" as="sourcePoint" />
        <mxPoint x="-759" y="1599.9999999999995" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-18" connectable="0" parent="autogen-152" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Đề xuất ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0469" y="-7" as="geometry">
        <mxPoint x="13" y="1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-19" connectable="0" parent="autogen-152" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8714" as="geometry">
        <mxPoint x="11" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-20" connectable="0" parent="autogen-152" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9109" y="-2" as="geometry">
        <mxPoint x="19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-155" edge="1" parent="1" source="autogen-34" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-470" y="2590" />
          <mxPoint x="790" y="2590" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-19" connectable="0" parent="autogen-155" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9605" as="geometry">
        <mxPoint x="13" y="-5" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-20" connectable="0" parent="autogen-155" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9765" y="-1" as="geometry">
        <mxPoint x="13" y="3" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-21" connectable="0" parent="autogen-155" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.3323" y="4" as="geometry">
        <mxPoint x="1" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-158" edge="1" parent="1" source="autogen-34" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="O05rKdK9spuyOT-CUAnu-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-14" connectable="0" parent="autogen-158" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8562" y="-5" as="geometry">
        <mxPoint x="10" y="-12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-15" connectable="0" parent="autogen-158" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9108" as="geometry">
        <mxPoint x="28" y="-13" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-17" connectable="0" parent="autogen-158" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=75;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Đề xuất ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.4107" y="-2" as="geometry">
        <mxPoint x="28" y="23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-161" edge="1" parent="1" source="autogen-34" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="5xCHvEqshszrvCJ6u1h0-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-76" connectable="0" parent="autogen-161" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7847" y="3" as="geometry">
        <mxPoint y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-77" connectable="0" parent="autogen-161" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7622" y="2" as="geometry">
        <mxPoint x="-7" y="-22" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-78" connectable="0" parent="autogen-161" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-35;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ dựa vào&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0116" y="-3" as="geometry">
        <mxPoint x="-15" y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-167" edge="1" parent="1" source="autogen-42" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="sSkXnrdGMHA7VeClHVL3-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-74" connectable="0" parent="autogen-167" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8322" y="-4" as="geometry">
        <mxPoint x="11" y="7" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-75" connectable="0" parent="autogen-167" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7326" y="-1" as="geometry">
        <mxPoint x="24" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-138" connectable="0" parent="autogen-167" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;Thuộc ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0899" y="-3" as="geometry">
        <mxPoint x="12" y="6" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-170" edge="1" parent="1" source="wVpjQ2P8cBYgM3cTY1Va-7" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-25" connectable="0" parent="autogen-170" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8023" y="-7" as="geometry">
        <mxPoint x="7" y="7" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-26" connectable="0" parent="autogen-170" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8936" y="-5" as="geometry">
        <mxPoint x="17" y="15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-27" connectable="0" parent="autogen-170" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-47;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;Thuộc ▶&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0241" y="-5" as="geometry">
        <mxPoint x="20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-179" edge="1" parent="1" source="xWTXjU9PBJvNTCzDRvnS-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="vUZi053HICLdzrY2CnxL-5">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="autogen-180" connectable="0" parent="autogen-179" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=20;" value="0..*" vertex="1">
      <mxGeometry relative="1" x="0.85" as="geometry">
        <mxPoint x="23" y="187" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ZYtPWBJm8RI_xscLBX3h-5" connectable="0" parent="autogen-179" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;1&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8326" as="geometry">
        <mxPoint x="12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ZYtPWBJm8RI_xscLBX3h-6" connectable="0" parent="autogen-179" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0626" y="-3" as="geometry">
        <mxPoint x="9" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-194" edge="1" parent="1" source="NvCRhkCUS0AH-Iu4hbs--1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="FGaaYxJouGr9IegEVFHG-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-47" connectable="0" parent="autogen-194" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8259" y="-3" as="geometry">
        <mxPoint x="9" y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-48" connectable="0" parent="autogen-194" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7116" y="-1" as="geometry">
        <mxPoint y="-23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-51" connectable="0" parent="autogen-194" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=30;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0914" y="-4" as="geometry">
        <mxPoint x="9" y="-10" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-197" edge="1" parent="1" source="NvCRhkCUS0AH-Iu4hbs--1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="LKcM5z-ubJVyyam-9tI6-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-82" connectable="0" parent="autogen-197" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8411" y="3" as="geometry">
        <mxPoint x="-1" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-83" connectable="0" parent="autogen-197" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8622" y="2" as="geometry">
        <mxPoint y="-23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-84" connectable="0" parent="autogen-197" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-15;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.3191" y="-1" as="geometry">
        <mxPoint x="-16" y="-23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-200" edge="1" parent="1" source="wVpjQ2P8cBYgM3cTY1Va-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="yhDT0zH1PYg4IR5jnH_h-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-114" connectable="0" parent="autogen-200" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7319" y="-4" as="geometry">
        <mxPoint x="18" y="1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-115" connectable="0" parent="autogen-200" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.5848" y="-4" as="geometry">
        <mxPoint x="21" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-116" connectable="0" parent="autogen-200" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-60;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.2402" y="-3" as="geometry">
        <mxPoint x="9" y="12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-203" edge="1" parent="1" source="y--h2MhFFpbY8mDH8gJM-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="5xCHvEqshszrvCJ6u1h0-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-34" connectable="0" parent="autogen-203" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7286" y="-1" as="geometry">
        <mxPoint x="-6" y="-21" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-35" connectable="0" parent="autogen-203" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.7426" y="-3" as="geometry">
        <mxPoint x="-27" y="-12" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-36" connectable="0" parent="autogen-203" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-50;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0567" y="-1" as="geometry">
        <mxPoint x="-19" y="-6" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-209" edge="1" parent="1" source="autogen-50" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="sSkXnrdGMHA7VeClHVL3-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-109" connectable="0" parent="autogen-209" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9658" y="5" as="geometry">
        <mxPoint x="20" y="-4" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-110" connectable="0" parent="autogen-209" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9599" as="geometry">
        <mxPoint x="15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-111" connectable="0" parent="autogen-209" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0422" y="1" as="geometry">
        <mxPoint x="14" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-212" edge="1" parent="1" source="autogen-50" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="O05rKdK9spuyOT-CUAnu-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-800" y="239" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-4" connectable="0" parent="autogen-212" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9734" y="1" as="geometry">
        <mxPoint y="-18" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="ob-vmPQV9zG-7l0FT_MR-5" connectable="0" parent="autogen-212" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9769" as="geometry">
        <mxPoint x="10" y="1" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-112" connectable="0" parent="autogen-212" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.2676" as="geometry">
        <mxPoint x="1" y="-19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-218" edge="1" parent="1" source="autogen-54" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="TLakgcBGVkkkUZ1dkReR-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1160" y="280" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-139" connectable="0" parent="autogen-218" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&amp;nbsp;&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9459" y="-3" as="geometry">
        <mxPoint x="27" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-140" connectable="0" parent="autogen-218" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9592" y="3" as="geometry">
        <mxPoint y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-141" connectable="0" parent="autogen-218" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0635" y="3" as="geometry">
        <mxPoint x="23" y="5" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-221" edge="1" parent="1" source="autogen-54" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-7" connectable="0" parent="autogen-221" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.7417" y="5" as="geometry">
        <mxPoint x="15" y="3" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-8" connectable="0" parent="autogen-221" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.8556" y="3" as="geometry">
        <mxPoint x="15" y="9" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-9" connectable="0" parent="autogen-221" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-50;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.0622" as="geometry">
        <mxPoint x="23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-236" edge="1" parent="1" source="LE_D31fqEWLUAxVKmXNx-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry" />
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-20" connectable="0" parent="autogen-236" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9623" as="geometry">
        <mxPoint x="1" y="-15" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-1" connectable="0" parent="autogen-236" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9328" as="geometry">
        <mxPoint x="12" y="-20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-5" connectable="0" parent="autogen-236" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;có ▶&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="0.0604" y="-1" as="geometry">
        <mxPoint x="-1" y="-17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-248" edge="1" parent="1" source="LcXY-409vzfgchgwM_3A-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="sSkXnrdGMHA7VeClHVL3-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="-2140" y="1493" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-66" connectable="0" parent="autogen-248" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.8205" as="geometry">
        <mxPoint x="14" y="31" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-67" connectable="0" parent="autogen-248" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9258" as="geometry">
        <mxPoint x="2" y="17" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-69" connectable="0" parent="autogen-248" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;◀ dựa vào&amp;nbsp;&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.2483" y="6" as="geometry">
        <mxPoint x="9" y="5" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-257" edge="1" parent="1" source="hnMNz6sLJDRykfnXpEXb-1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="xWTXjU9PBJvNTCzDRvnS-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="2240" y="190" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-123" connectable="0" parent="autogen-257" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9348" as="geometry">
        <mxPoint x="30" y="34" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-124" connectable="0" parent="autogen-257" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9652" y="-1" as="geometry">
        <mxPoint y="-19" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-125" connectable="0" parent="autogen-257" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ thuộc&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.3238" y="3" as="geometry">
        <mxPoint x="1" y="-23" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-266" edge="1" parent="1" source="autogen-58" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="enBTkT7pxzC_SMcUWDWl-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1370" y="1440" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-10" connectable="0" parent="autogen-266" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9464" y="-4" as="geometry">
        <mxPoint x="-1" y="-13" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-11" connectable="0" parent="autogen-266" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&amp;nbsp;&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.925" y="-1" as="geometry">
        <mxPoint x="31" y="-13" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="9OtpY_rXGSmW5QJFDCLS-12" connectable="0" parent="autogen-266" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ có&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.3493" y="-2" as="geometry">
        <mxPoint x="22" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="autogen-269" edge="1" parent="1" source="autogen-58" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;edgeStyle=orthogonalEdgeStyle;" target="TLakgcBGVkkkUZ1dkReR-1">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1360" y="320" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-144" connectable="0" parent="autogen-269" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9342" y="-1" as="geometry">
        <mxPoint y="-21" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-145" connectable="0" parent="autogen-269" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.9429" y="-3" as="geometry">
        <mxPoint x="17" y="-9" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-148" connectable="0" parent="autogen-269" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ đề xuất&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.1912" as="geometry">
        <mxPoint x="20" y="11" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="Jqhau5jHDxXsKYdf8Zkh-1" parent="1" style="swimlane;fontStyle=0;align=center;verticalAlign=top;childLayout=stackLayout;horizontal=1;startSize=26;fillColor=none;horizontalStack=0;resizeParent=1;resizeParentMax=0;resizeLast=0;collapsible=1;marginBottom=0;whiteSpace=wrap;html=1;" value="NhatKyPhuTrachDuAn" vertex="1">
      <mxGeometry height="124" width="220" x="834" y="650" as="geometry">
        <mxRectangle height="30" width="120" x="-205" y="830.0000000000001" as="alternateBounds" />
      </mxGeometry>
    </mxCell>
    <mxCell id="Jqhau5jHDxXsKYdf8Zkh-2" parent="Jqhau5jHDxXsKYdf8Zkh-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ MaNhatKyPTDA: int&lt;br&gt;+ NkHanhDongPTDA: string&lt;br&gt;+ NkThoiGianPTDA: DateTime" vertex="1">
      <mxGeometry height="64" width="220" y="26" as="geometry" />
    </mxCell>
    <mxCell id="Jqhau5jHDxXsKYdf8Zkh-3" parent="Jqhau5jHDxXsKYdf8Zkh-1" style="line;strokeWidth=1;fillColor=none;align=left;verticalAlign=middle;spacingTop=-1;spacingLeft=3;spacingRight=3;rotatable=0;labelPosition=right;points=[];portConstraint=eastwest;strokeColor=inherit;" value="" vertex="1">
      <mxGeometry height="8" width="220" y="90" as="geometry" />
    </mxCell>
    <mxCell id="Jqhau5jHDxXsKYdf8Zkh-4" parent="Jqhau5jHDxXsKYdf8Zkh-1" style="text;strokeColor=none;fillColor=none;align=left;verticalAlign=top;spacingLeft=4;spacingRight=4;overflow=hidden;rotatable=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;whiteSpace=wrap;html=1;" value="+ ghiLog(): void" vertex="1">
      <mxGeometry height="26" width="220" y="98" as="geometry" />
    </mxCell>
    <mxCell id="ZYtPWBJm8RI_xscLBX3h-7" edge="1" parent="1" style="rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;dashed=1;" target="autogen-46">
      <mxGeometry relative="1" as="geometry">
        <mxPoint x="-1865" y="2240" as="sourcePoint" />
        <mxPoint x="-1498" y="2633" as="targetPoint" />
      </mxGeometry>
    </mxCell>
    <mxCell id="93-mJI576MT82vWcGMnV-3" edge="1" parent="1" source="6wtPaquReI3S4elhgb0T-4" style="edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=none;endFill=0;" target="autogen-58">
      <mxGeometry relative="1" as="geometry">
        <Array as="points">
          <mxPoint x="1280" y="-120" />
        </Array>
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-146" connectable="0" parent="93-mJI576MT82vWcGMnV-3" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;1&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.93" y="2" as="geometry">
        <mxPoint y="-16" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-147" connectable="0" parent="93-mJI576MT82vWcGMnV-3" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];" value="&lt;font style=&quot;font-size: 20px;&quot;&gt;0..*&lt;/font&gt;" vertex="1">
      <mxGeometry relative="1" x="0.9405" as="geometry">
        <mxPoint x="20" as="offset" />
      </mxGeometry>
    </mxCell>
    <mxCell id="G3jGO_3h6dEc7QatDs8r-149" connectable="0" parent="93-mJI576MT82vWcGMnV-3" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];rotation=-90;" value="&lt;span style=&quot;font-size: 20px;&quot;&gt;◀ duyệt&amp;nbsp;&lt;/span&gt;" vertex="1">
      <mxGeometry relative="1" x="-0.1276" y="3" as="geometry">
        <mxPoint x="17" as="offset" />
      </mxGeometry>
    </mxCell>
  </root>
</mxGraphModel>

SELECT      TOP (200) Id, Name, DisplayOrder
FROM          SpecificationAttribute
WHERE      (Name IN ('��� �������', '��������� ����', '�������������', '����� ��� ��������� ��������', '�������������', 
                        '������������� ������������������', '������� �������', '�������� �������'))
ORDER BY DisplayOrder
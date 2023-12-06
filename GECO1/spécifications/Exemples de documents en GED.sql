use gedmaf;

select top 10
	ID_GED
from ARCHEAMAF
where CODE_PIECE='GECO1'
and LIB_DOC like '%signé'
order by docn desc
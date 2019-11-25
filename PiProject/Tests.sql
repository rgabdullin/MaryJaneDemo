select
	*
from
	dbo.V12_Measurement
order by Id desc;

select
	*
from
	dbo.V12_Label;

select
	*
from
	dbo.V12_SpectralData
where 
	MeasurementId = 181
order by
	Channel;
select 
	dataset.Id,
	dataset.Name,
	count(*)
from
	dbo.V12_Dataset as dataset
join
	dbo.V12_Measurement as meas on dataset.Id = meas.DatasetId
group by
	dataset.Id, dataset.Name;
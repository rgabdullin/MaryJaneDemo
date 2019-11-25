select
	meas.Id as Id,
	label.Name as Label,
	dataset.Name as Dataset,
	spec.Channel as Channel,
	Value as Value
from
	dbo.V12_SpectralData as spec
join
	dbo.V12_Measurement as meas on spec.MeasurementId = meas.Id 
join 
	dbo.V12_Dataset as dataset on meas.DatasetId = dataset.Id
left join
	dbo.V12_Label as label on meas.LabelId = label.Id
where
	meas.IsTrain = 1;
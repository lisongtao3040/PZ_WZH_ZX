
declare @startDate datetime;
declare @endDate datetime;
declare @curDate datetime;
set @endDate = dateadd(month,-18,getdate());
set @startDate = (select min(ins_date) from [wzh_new].[dbo].[m_picture_chk]);
set @curDate = @startDate;


if @startDate<@endDate
begin
	print '有要备份的数据';

	set @curDate = @startDate;
	while @curDate<@endDate
	begin
		print @curDate;
		--1.备份数据
		insert into [wzh_new_over_one_year_bk].[dbo].[m_picture_chk]
		select * from [wzh_new].[dbo].[m_picture_chk]
		where ins_date < @curDate;


		IF @@ROWCOUNT > 0
		BEGIN
 			PRINT cast(@curDate as varchar(40)) + 'm_picture_chk备份数据成功！';
			--删除数据
			delete from [wzh_new].[dbo].[m_picture_chk]
			where ins_date < @curDate;

		END
		ELSE
		BEGIN
			-- 备份失败，执行相应的错误处理逻辑
			PRINT cast(@curDate as varchar(40)) + 'm_picture_chk备份数据失败！';
		END

		set @curDate = dateadd(day,1,@curDate);
	end 

end

--t_check_ms
set @startDate = (select min(upd_date) from [wzh_new].[dbo].t_check_ms);
set @curDate = @startDate;

if @startDate<@endDate
begin

	set @curDate = @startDate;
	while @curDate<@endDate
	begin
		print @curDate;
		--1.备份数据
		insert into [wzh_new_over_one_year_bk].[dbo].[t_check_ms]
		select * from [wzh_new].[dbo].[t_check_ms]
		where upd_date < @curDate;


		IF @@ROWCOUNT > 0
		BEGIN
 			PRINT cast(@curDate as varchar(40)) + 't_check_ms备份数据成功！';
			--删除数据
			delete from [wzh_new].[dbo].[t_check_ms]
			where upd_date < @curDate;

		END
		ELSE
		BEGIN
			-- 备份失败，执行相应的错误处理逻辑
			PRINT cast(@curDate as varchar(40)) + 't_check_ms备份数据失败！';
		END

		set @curDate = dateadd(day,1,@curDate);
	end 

end
